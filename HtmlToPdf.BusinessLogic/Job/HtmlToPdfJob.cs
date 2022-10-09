using HtmlToPdf.DAO;
using PuppeteerSharp;

namespace HtmlToPdf.BusinessLogic.Job;

public class HtmlToPdfJob : ConvertJob
{
    public HtmlToPdfJob(IMediaFilesRepository mediaFilesRepository) : base(mediaFilesRepository)
    {
    }

    protected override async Task Convert(string inputPath, string outputPath)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(inputPath);
        await page.PdfAsync(outputPath);
        
        await page.CloseAsync();
        await browser.CloseAsync();
    }

    protected override string GetExtension() => "pdf";
    protected override string GetContentType() => "application/pdf";
}