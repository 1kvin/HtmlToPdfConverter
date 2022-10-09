using HtmlToPdf.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HtmlToPdf.DI;

public static class DatabaseDI
{
    public static void AddDatabaseDI(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<MediaFilesDbContext>(opt => opt.UseInMemoryDatabase("db"));

        serviceCollection.AddTransient<IMediaFilesRepository, EfMediaFilesRepository>();
    }
}