using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HtmlToPdf.Host.Controllers;

public class MediaFileController : Controller
{
    private readonly IMediaFileConvertService mediaFileConvertService;

    public MediaFileController(IMediaFileConvertService mediaFileConvertService)
    {
        this.mediaFileConvertService = mediaFileConvertService;
    }

    [HttpGet]
    public async Task<IActionResult> Download(string jobId)
    {
        try
        {
            var convertResult = await mediaFileConvertService.GetResult(jobId);

            if (convertResult == null)
            {
                return NotFound("The file is not ready yet");
            }
        
            return PhysicalFile(convertResult.Path, convertResult.ContentType, convertResult.FileName);
        }
        catch (JobNotFoundException)
        {
            return NotFound($"Job {jobId} not found");
        }
        catch (FileNotFoundException)
        {
            return NotFound($"File not found");
        }
    }
}