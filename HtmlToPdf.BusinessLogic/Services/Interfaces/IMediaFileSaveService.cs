using Microsoft.AspNetCore.Http;

namespace HtmlToPdf.BusinessLogic.Services.Interfaces;

public interface IMediaFileSaveService
{
    public Task<int> SaveFromLocalToDb(IFormFile file);
    public Task<string> SaveFromDbToLocal(int id);
}