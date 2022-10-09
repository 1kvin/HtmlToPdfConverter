using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Mvc;

namespace HtmlToPdf.Host.Controllers;

public class HomeController : Controller
{
    private readonly IMediaFileConvertService mediaFileConvertService;

    public HomeController(IMediaFileConvertService mediaFileConvertService)
    {
        this.mediaFileConvertService = mediaFileConvertService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile? files)
    {
        try
        {
            var jobId = await mediaFileConvertService.Convert(files, MediaFileType.PDF);

            return RedirectToAction(nameof(Progress), new { jobId });
        }
        catch (FileSizeException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotSupportedException e)
        {
            return BadRequest(e.Message);
        }
    }

    public IActionResult Progress(string jobId)
    {
        ViewBag.JobId = jobId;

        return View();
    }
}