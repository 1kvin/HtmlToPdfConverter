using Hangfire;
using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Job;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HtmlToPdf.BusinessLogic.Services.Implementations;

public class MediaFileConvertService : IMediaFileConvertService
{
    private readonly IBackgroundJobClient jobClient;
    private readonly IMediaFileSaveService mediaFileSaveService;
    private readonly IMediaFilesRepository mediaFilesRepository;

    public MediaFileConvertService(IBackgroundJobClient jobClient, IMediaFileSaveService mediaFileSaveService,
        IMediaFilesRepository mediaFilesRepository)
    {
        this.jobClient = jobClient;
        this.mediaFileSaveService = mediaFileSaveService;
        this.mediaFilesRepository = mediaFilesRepository;
    }

    public async Task<string> Convert(IFormFile? file, MediaFileType toFormat)
    {
        if (file == null)
        {
            throw new FileNotFoundException();
        }

        var fromFormat = ExtensionToMediaFileType.Convert(file);

        var converter = GetConverter(fromFormat, toFormat);

        if (converter == null)
        {
            throw new NotSupportedException($"No converter found for this file");
        }

        var fileId = await mediaFileSaveService.SaveFromLocalToDb(file);

        return converter.Invoke(fileId);
    }

    public async Task<ConvertResult?> GetResult(string jobId)
    {
        var jobMonitoringApi = JobStorage.Current.GetMonitoringApi();
        var job = jobMonitoringApi.JobDetails(jobId);

        if (job == null)
        {
            throw new JobNotFoundException();
        }

        var succeededState = job.History.SingleOrDefault(x => x.StateName == "Succeeded");

        if (succeededState == null)
        {
            return null;
        }

        var jobResultId = succeededState.Data["Result"];
        var mediaFileId = int.Parse(jobResultId);


        var path = await mediaFileSaveService.SaveFromDbToLocal(mediaFileId);

        var mediaFileInDb = await mediaFilesRepository.MediaFiles.SingleAsync(x => x.Id == mediaFileId);

        return new ConvertResult
        {
            Path = path,
            FileName = mediaFileInDb.Name,
            ContentType = mediaFileInDb.ContentType!
        };
    }

    private Func<int, string>? GetConverter(MediaFileType fromFormat, MediaFileType toFormat)
    {
        if (fromFormat == MediaFileType.HTML && toFormat == MediaFileType.PDF)
        {
            return id => jobClient.Enqueue<HtmlToPdfJob>(x => x.Convert(id));
        }

        return null;
    }
}