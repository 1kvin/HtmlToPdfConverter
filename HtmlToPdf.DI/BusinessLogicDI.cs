using HtmlToPdf.BusinessLogic.Services.Implementations;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlToPdf.DI;

public static class BusinessLogicDI
{
    public static void AddBusinessLogicDI(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IMediaFileSaveService, MediaFileSaveService>();
        serviceCollection.AddTransient<IMediaFileConvertService, MediaFileConvertService>();
        serviceCollection.AddTransient<IJobStatusService, JobStatusService>();
    }
}