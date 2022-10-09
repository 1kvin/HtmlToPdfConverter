using HtmlToPdf.Models;
using Microsoft.EntityFrameworkCore;

namespace HtmlToPdf.DAO;

public interface IMediaFilesRepository
{
    DbSet<MediaFile> MediaFiles { get; }

    void SaveChanges();
    Task SaveChangesAsync();
}