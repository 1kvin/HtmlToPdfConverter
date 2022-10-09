using HtmlToPdf.BusinessLogic.Job;
using HtmlToPdf.DAO;

namespace HtmlToPdf.UnitTests.Job;

public class ConvertJobSpy : ConvertJob
{
    public bool IsConvert { get; private set; } = false;

    private readonly string extension;
    private readonly string contentType;

    public ConvertJobSpy(string extension, string contentType, IMediaFilesRepository mediaFilesRepository) : base(
        mediaFilesRepository)
    {
        this.extension = extension;
        this.contentType = contentType;
    }

    protected override Task Convert(string inputPath, string outputPath)
    {
        IsConvert = true;
        
        return Task.CompletedTask;
    }

    protected override string GetExtension() => extension;

    protected override string GetContentType() => contentType;
}