using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;

namespace HtmlToPdf.BusinessLogic.Services.Implementations;

public class MediaFileSaveService : IMediaFileSaveService
{
    public const int MaxFileSize = 512  * 1024 * 1024;
    private readonly IMediaFilesRepository mediaFilesRepository;

    public MediaFileSaveService(IMediaFilesRepository mediaFilesRepository)
    {
        this.mediaFilesRepository = mediaFilesRepository;
    }

    public async Task<int> Save(IFormFile file)
    {
        switch (file.Length)
        {
            case > MaxFileSize:
                throw new FileSizeException("File too large");
            case <= 0:
                throw new FileSizeException("Empty file size");
        }


        var fileName = Path.GetFileName(file.FileName);
        var fileExtension = Path.GetExtension(fileName);

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
}