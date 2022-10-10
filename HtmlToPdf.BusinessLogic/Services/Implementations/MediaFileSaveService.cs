using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.BusinessLogic.Services.Implementations;

public class MediaFileSaveService : IMediaFileSaveService
{
    public const int MaxFileSize = 512  * 1024 * 1024;
    private readonly IMediaFilesRepository mediaFilesRepository;
    private readonly string basePath;

    public MediaFileSaveService(IMediaFilesRepository mediaFilesRepository)
    {
        this.mediaFilesRepository = mediaFilesRepository;
        basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
    }

    public async Task<int> SaveFromLocalToDb(IFormFile file)
    {
        switch (file.Length)
        {
            case > MaxFileSize:
                throw new FileSizeException("File too large");
            case <= 0:
                throw new FileSizeException("Empty file size");
        }


        var fileName = Path.GetFileName(file.FileName);
        var fileExtension = Path.GetExtension(fileName).Replace(".", "");

        using var target = new MemoryStream();

        await file.CopyToAsync(target);

        var mediaFile = new MediaFile
        {
            Name = fileName,
            Type = ExtensionToMediaFileType.Convert(fileExtension),
            Extension = fileExtension,
            UploadDate = DateTime.UtcNow,
            Content = target.ToArray()
        };

        await mediaFilesRepository.MediaFiles.AddAsync(mediaFile);
        await mediaFilesRepository.SaveChangesAsync();

        return mediaFile.Id;
    }

    public async Task<string> SaveFromDbToLocal(int id)
    {
        var mediaFileInDb = await mediaFilesRepository.MediaFiles.SingleOrDefaultAsync(x => x.Id == id);
        
        if (mediaFileInDb == null)
        {
            throw new FileNotFoundException();
        }
        
        var outputPath = Path.Combine(basePath, PathUtil.saveOutputDirectory, $"{mediaFileInDb.Id}.{mediaFileInDb.Extension}");

        if (File.Exists(outputPath))
        {
            return outputPath;
        }
        
        CreateDirectoryIfNotExist(outputPath);
        await File.WriteAllBytesAsync(outputPath, mediaFileInDb.Content);

        return outputPath;
    }
    
    private static void CreateDirectoryIfNotExist(string filePath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        }
    }
}