using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.BusinessLogic.Job;

public abstract class ConvertJob
{
    private readonly string basePath;
    private readonly IMediaFilesRepository mediaFilesRepository;

    protected ConvertJob(IMediaFilesRepository mediaFilesRepository)
    {
        this.mediaFilesRepository = mediaFilesRepository;

        basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
    }
    
    public async Task<ConvertResult> Convert(int mediaFileId)
    {
        var mediaFile = await mediaFilesRepository.MediaFiles.SingleAsync(x => x.Id == mediaFileId);

        var inputPath = Path.Combine(basePath, PathUtil.saveInputDirectory, mediaFile.Id.ToString()) + mediaFile.Extension;
        CreateDirectoryIfNotExist(inputPath);
       
        await File.WriteAllBytesAsync(inputPath, mediaFile.Content);
        
        
        var outputPath = Path.Combine(basePath, PathUtil.saveOutputDirectory, $"{mediaFile.Id}.{GetExtension()}");
        CreateDirectoryIfNotExist(outputPath);
        
        
        await Convert(inputPath, outputPath);
        
        return new ConvertResult
        {
            Path = outputPath,
            ContentType = GetContentType(),
            FileName = $"{mediaFile.Name}.{GetExtension()}"
        };
    }

    protected abstract Task Convert(string inputPath, string outputPath);
    protected abstract string GetExtension();
    protected abstract string GetContentType();

    private static void CreateDirectoryIfNotExist(string filePath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        }
    }
}