using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;

namespace HtmlToPdf.BusinessLogic.Utilities;

public static class ExtensionToMediaFileType
{
    public static MediaFileType Convert(IFormFile file)
    {
        var fileName = Path.GetFileName(file.FileName);
        var fileExtension = Path.GetExtension(fileName);

        return Convert(fileExtension);
    }
    
    public static MediaFileType Convert(string extension)
    {
        if (!extension.StartsWith('.'))
        {
            extension = "." + extension;
        }
        
        return extension.ToLower() switch
        {
            ".html" => MediaFileType.HTML,
            ".pdf" => MediaFileType.PDF,
            _ => throw new NotSupportedException("Cant convert this format")
        };
    }
}