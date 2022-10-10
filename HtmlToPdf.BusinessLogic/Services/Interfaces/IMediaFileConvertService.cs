using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;

namespace HtmlToPdf.BusinessLogic.Services.Interfaces;

public interface IMediaFileConvertService
{
    public Task<string> Convert(IFormFile? file, MediaFileType toFormat);
    public Task<ConvertResult?> GetResult(string jobId);
}