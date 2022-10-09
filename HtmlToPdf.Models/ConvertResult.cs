namespace HtmlToPdf.Models;

public class ConvertResult
{
    public string Path { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string? FileName { get; set; }
}