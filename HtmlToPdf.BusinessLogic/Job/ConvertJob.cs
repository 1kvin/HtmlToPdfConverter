using HtmlToPdf.BusinessLogic.Exceptions;
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
    
    public async Task<int> Convert(int mediaFileId)
    {
        var mediaFile = await mediaFilesRepository.MediaFiles.SingleAsync(x => x.Id == mediaFileId);

        var inputPath = Path.Combine(basePath, PathUtil.saveInputDirectory, mediaFile.Id.ToString()) + "." + mediaFile.Extension;
        CreateDirectoryIfNotExist(inputPath);
       
        await File.WriteAllBytesAsync(inputPath, mediaFile.Content);


        var extension = GetExtension();
        
        var outputPath = Path.Combine(basePath, PathUtil.saveOutputDirectory, $"{mediaFile.Id}.{extension}");
        CreateDirectoryIfNotExist(outputPath);
        
        
        await Convert(inputPath, outputPath);

        if (!File.Exists(outputPath))
        {
            throw new ConvertException();
        }
        
        var content = await File.ReadAllBytesAsync(outputPath);


        var saveMediaFile = new MediaFile
        {
            Content = content,
            UploadDate = DateTime.UtcNow,
            Type = ExtensionToMediaFileType.Convert(extension),
            Name = $"{mediaFile.Name}.{extension}",
            Extension = extension,
            ContentType = GetContentType()
        };

        await mediaFilesRepository.MediaFiles.AddAsync(saveMediaFile);
        await mediaFilesRepository.SaveChangesAsync();

        
        File.Delete(inputPath);
        File.Delete(outputPath);

        return saveMediaFile.Id;
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