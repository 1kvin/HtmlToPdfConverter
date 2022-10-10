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
        
        Assert.True(result.Content.Length != 0);
    }

    [Test]
    public async Task CompareWithExistsFileTest()
    {
        var content = await File.ReadAllBytesAsync("TestData/Job/HtmlToPdfJob/1.html");

        var result = await MockJobAndConvert(content);
        Assert.True(result.Content.Length != 0);

        var saveFilePath = "TestData/1.pdf";
        
        await File.WriteAllBytesAsync(saveFilePath, result.Content);
        
        
        var resultPdf = ReadPdfFile(saveFilePath);
        var expectedPdf = ReadPdfFile("TestData/Job/HtmlToPdfJob/-1.pdf");
        
        File.Delete(saveFilePath);
        
        Assert.That(resultPdf, Is.EqualTo(expectedPdf));
    }

    private async Task<MediaFile> MockJobAndConvert(byte[] content)
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
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles.AddAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()))
            .Callback<MediaFile, CancellationToken>((file, _) => files.Add(file));

        var job = new HtmlToPdfJob(mediaFilesRepositoryMock.Object);

        var resultId = await job.Convert(id);

        return files.Single(x => x.Id == resultId);
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