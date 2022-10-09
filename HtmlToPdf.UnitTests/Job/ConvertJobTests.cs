using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Moq;
using Moq.EntityFrameworkCore;

namespace HtmlToPdf.UnitTests.Job;

public class ConvertJobTests
{
    private const string inputExtension = ".html";
    private const string fileName = "abcde" + inputExtension;

    private const string outputExtension = "pdf";
    private const string contentType = "application/pdf";
    private const int id = -1;

    [Test]
    public async Task OutputParametersEqualInputTest()
    {
        var files = new List<MediaFile>
        {
            new()
            {
                Id = id,
                Extension = inputExtension,
                Name = fileName,
                Content = new byte[10]
            }
        };

        var mediaFilesRepositoryMock = new Mock<IMediaFilesRepository>();
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles)
            .ReturnsDbSet(files);


        var convertJob = new ConvertJobSpy(outputExtension, contentType, mediaFilesRepositoryMock.Object);

        var result = await convertJob.Convert(id);
        Assert.That(convertJob.IsConvert, Is.True);
        Assert.That(result.ContentType, Is.EqualTo(contentType));
        Assert.That(result.FileName, Is.EqualTo(fileName + "." + outputExtension));
    }
}