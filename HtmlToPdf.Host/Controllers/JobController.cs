using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HtmlToPdf.Host.Controllers;

public class JobController : Controller
{
    private readonly IJobStatusService jobStatusService;

    public JobController(IJobStatusService jobStatusService)
    {
        this.jobStatusService = jobStatusService;
    }

    [HttpGet]
    public IActionResult GetStatus(string jobId)
    {
        try
        {
            return Ok(jobStatusService.GetJobStatus(jobId));
        }
        catch (JobNotFoundException)
        {
            return NotFound($"Job {jobId} not found");
        }
    }
}