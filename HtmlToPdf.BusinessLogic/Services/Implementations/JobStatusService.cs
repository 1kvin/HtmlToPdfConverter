using Hangfire;
using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Interfaces;

namespace HtmlToPdf.BusinessLogic.Services.Implementations;

public class JobStatusService : IJobStatusService
{
    public string GetJobStatus(string jobId)
    {
        var connection = JobStorage.Current.GetConnection();
        var jobData = connection.GetJobData(jobId);

        if (jobData == null)
        {
            throw new JobNotFoundException();
        }
        
        return jobData.State;
    }
}