using HtmlToPdf.Models;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.DAO;

public class MediaFilesDbContext : DbContext
{
    public MediaFilesDbContext(DbContextOptions<MediaFilesDbContext> options) : base(options)
    {
    }
    public DbSet<MediaFile> MediaFiles { get; set; } = null!;
}