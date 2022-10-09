using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlToPdf.DI;

public static class JobDI
{
    public static void AddJobDI(this IServiceCollection services)
    {
        services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
            config.UseConsole();
        });
        
        services.AddHangfireConsoleExtensions();
        services.AddHangfireServer();
    }
}