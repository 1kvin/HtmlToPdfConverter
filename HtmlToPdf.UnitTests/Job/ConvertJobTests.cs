using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using MockQueryable.Moq;
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
    public void OutputFileNotFoundTest()
    {
        Assert.ThrowsAsync<ConvertException>(async () =>  await MockAndRunConvert(false));
    }
    
    [Test]
    public async Task CreateFileTest()
    {
        var result = await MockAndRunConvert(true);
        Assert.That(result.ContentType, Is.EqualTo(contentType));
        Assert.That(result.Name, Is.EqualTo(fileName + "." + outputExtension));
    }

    private async Task<MediaFile> MockAndRunConvert(bool createFile)
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
        
        var mock = files.AsQueryable().BuildMockDbSet();
        
        
        var mediaFilesRepositoryMock = new Mock<IMediaFilesRepository>();
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles)
            .Returns(mock.Object);
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles.AddAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()))
            .Callback<MediaFile, CancellationToken>((file, _) => files.Add(file));


        var convertJob = new FakeConvertJob(outputExtension, contentType, createFile, mediaFilesRepositoryMock.Object);
        
        var resultId = await convertJob.Convert(id);

        return files.Single(x => x.Id == resultId);
    }
}