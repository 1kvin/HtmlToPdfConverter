using HtmlToPdf.Models;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.DAO;

public class EfMediaFilesRepository : IMediaFilesRepository
{
    private readonly MediaFilesDbContext mediaFilesDbContext;

    public EfMediaFilesRepository(MediaFilesDbContext mediaFilesDbContext)
    {
        this.mediaFilesDbContext = mediaFilesDbContext;
    }

    public DbSet<MediaFile> MediaFiles => mediaFilesDbContext.MediaFiles;

    public void SaveChanges()
    {
        mediaFilesDbContext.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await mediaFilesDbContext.SaveChangesAsync();
    }
}