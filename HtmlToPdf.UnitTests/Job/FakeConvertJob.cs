using HtmlToPdf.BusinessLogic.Job;
using HtmlToPdf.DAO;

namespace HtmlToPdf.UnitTests.Job;

public class FakeConvertJob : ConvertJob
{
    public bool IsConvert { get; private set; } = false;

    private readonly string extension;
    private readonly string contentType;
    private readonly bool createFile;

    public FakeConvertJob(string extension, string contentType, bool createFile, IMediaFilesRepository mediaFilesRepository) : base(
        mediaFilesRepository)
    {
        this.extension = extension;
        this.contentType = contentType;
        this.createFile = createFile;
    }

    protected override Task Convert(string inputPath, string outputPath)
    {
        IsConvert = true;
        
        if(createFile)
            File.WriteAllText(outputPath, "123");
        
        return Task.CompletedTask;
    }

    protected override string GetExtension() => extension;

    protected override string GetContentType() => contentType;
}