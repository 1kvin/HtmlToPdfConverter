using HtmlToPdf.BusinessLogic.Exceptions;
using HtmlToPdf.BusinessLogic.Services.Implementations;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.EntityFrameworkCore;

namespace HtmlToPdf.UnitTests.Services;

public class MediaFileSaveServiceTests
{
    private IMediaFileSaveService mediaFileSaveService;

    private const string testFileExtension = ".pdf";
    private const string testFileName = "name" + testFileExtension;

    [SetUp]
    public void SetUp()
    {
        var mediaFilesRepositoryMock = new Mock<IMediaFilesRepository>();
        mediaFilesRepositoryMock
            .Setup(x => x.MediaFiles)
            .ReturnsDbSet(Enumerable.Empty<MediaFile>());
        
        mediaFileSaveService = new MediaFileSaveService(mediaFilesRepositoryMock.Object);
    }

    [Test]
    public void EmptyFileTest()
    {
        Assert.ThrowsAsync<FileSizeException>(async () => await SizeTest(0));
    }

    [Test]
    public void BigSizeFileTest()
    {
        Assert.ThrowsAsync<FileSizeException>(async () => await SizeTest(MediaFileSaveService.MaxFileSize + 1));
    }

    [Test]
    public async Task GoodFileSizeTest()
    {
        await SizeTest(MediaFileSaveService.MaxFileSize / 2);
    }
    

    private async Task SizeTest(int size)
    {
        var fileStub = new Mock<IFormFile>();
        fileStub.Setup(x => x.Length).Returns(size);
        fileStub.Setup(x => x.FileName).Returns(testFileName);

        await mediaFileSaveService.SaveFromLocalToDb(fileStub.Object);
    }
}