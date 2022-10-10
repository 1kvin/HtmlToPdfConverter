using Hangfire;
using HtmlToPdf.BusinessLogic.Services.Implementations;
using HtmlToPdf.BusinessLogic.Services.Interfaces;
using HtmlToPdf.DAO;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HtmlToPdf.UnitTests.Services;

public class MediaFileConvertServiceTests
{
    private readonly IMediaFileConvertService mediaFileConvertService =
        new MediaFileConvertService(Mock.Of<IBackgroundJobClient>(), Mock.Of<IMediaFileSaveService>(),
            Mock.Of<IMediaFilesRepository>());

    private readonly Mock<IFormFile> fileStub = new();

    [Test]
    public void EmptyFileTest()
    {
        Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await mediaFileConvertService.Convert(null, MediaFileType.PDF));
    }

    [Test]
    public void UnknownFormatTest()
    {
        fileStub.Setup(x => x.FileName).Returns("1.abcdef");

        Assert.ThrowsAsync<NotSupportedException>(async () =>
            await mediaFileConvertService.Convert(fileStub.Object, MediaFileType.PDF));
    }

    [Test]
    public void NotFoundConverterTest()
    {
        fileStub.Setup(x => x.FileName).Returns("1.pdf");

        Assert.ThrowsAsync<NotSupportedException>(async () =>
            await mediaFileConvertService.Convert(fileStub.Object, MediaFileType.PDF));
    }

    [Test]
    public async Task GoodCaseTest()
    {
        fileStub.Setup(x => x.FileName).Returns("1.html");

        await mediaFileConvertService.Convert(fileStub.Object, MediaFileType.PDF);
    }
}