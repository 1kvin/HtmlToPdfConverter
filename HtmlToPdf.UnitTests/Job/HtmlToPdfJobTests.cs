using System.Text;
using HtmlToPdf.BusinessLogic.Job;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Moq;
using Moq.EntityFrameworkCore;

namespace HtmlToPdf.UnitTests.Job;

public class HtmlToPdfJobTests
{
    private const string inputExtension = ".html";
    private const string fileName = "abcde" + inputExtension;
    private const int id = -1;

    [Test]
    public async Task FileExistsTest()
    {
        var result = await MockJobAndConvert(new byte[10]);
        
        Assert.True(File.Exists(result.Path));
    }

    [Test]
    public async Task CompareWithExistsFileTest()
    {
        var content = await File.ReadAllBytesAsync("TestData/Job/HtmlToPdfJob/1.html");

        var result = await MockJobAndConvert(content);
        Assert.True(File.Exists(result.Path));


        var resultPdf = ReadPdfFile(result.Path);
        var expectedPdf = ReadPdfFile("TestData/Job/HtmlToPdfJob/-1.pdf");
        Assert.That(resultPdf, Is.EqualTo(expectedPdf));
    }

    private async Task<ConvertResult> MockJobAndConvert(byte[] content)
    {
        var files = new List<MediaFile>
        {
            new()
            {
                Id = id,
                Extension = inputExtension,
                Name = fileName,
                Content = content
            }
        };

        var mediaFilesRepositoryMock = new Mock<IMediaFilesRepository>();
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles)
            .ReturnsDbSet(files);

        var job = new HtmlToPdfJob(mediaFilesRepositoryMock.Object);

        return await job.Convert(id);
    }

    private string ReadPdfFile(string filePath)
    {
        var text = new StringBuilder();

        if (File.Exists(filePath))
        {
            var pdfReader = new PdfReader(filePath);

            var pdfDocument = new PdfDocument(pdfReader);
            
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                var page = pdfDocument.GetPage(i);
                var currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                text.Append(currentText);
            }

            pdfReader.Close();
        }

        return text.ToString();
    }
}