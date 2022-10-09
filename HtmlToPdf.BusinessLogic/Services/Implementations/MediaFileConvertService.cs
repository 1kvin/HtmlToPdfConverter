using Hangfire;
using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Job;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HtmlToPdf.BusinessLogic.Services.Implementations;

public class MediaFileConvertService : IMediaFileConvertService
{
    private readonly IBackgroundJobClient jobClient;
    private readonly IMediaFileSaveService mediaFileSaveService;

    public MediaFileConvertService(IBackgroundJobClient jobClient, IMediaFileSaveService mediaFileSaveService)
    {
        this.jobClient = jobClient;
        this.mediaFileSaveService = mediaFileSaveService;
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
        
        var fileId = await mediaFileSaveService.Save(file);

        return converter.Invoke(fileId);
    }

    private Func<int, string>? GetConverter(MediaFileType fromFormat, MediaFileType toFormat)
    {
        if (fromFormat == MediaFileType.HTML && toFormat == MediaFileType.PDF)
        {
            return id => jobClient.Enqueue<HtmlToPdfJob>(x => x.Convert(id));
        }

        return null;
    }
    

    public ConvertResult? GetResult(string jobId)
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

        var jobResult = succeededState.Data["Result"];
        return JsonConvert.DeserializeObject<ConvertResult>(jobResult)!;
    }
}