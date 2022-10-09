using HtmlToPdf.BusinessLogic.Utilities;
using HtmlToPdf.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HtmlToPdf.UnitTests.Utilities;

public class ExtensionToMediaFileTypeTests
{
    [Test]
    public void ConvertUnknownFormatFromStringTest()
    {
        Assert.Throws<NotSupportedException>(() => ExtensionToMediaFileType.Convert(".abcdef"));
    }

    [Test]
    public void ConvertStingExtensionWithoutDotTest()
    {
        Assert.Throws<NotSupportedException>(() => ExtensionToMediaFileType.Convert("pdf"));
    }

    [Test]
    public void ConvertGoodStingExtensionTest()
    {
        var result = ExtensionToMediaFileType.Convert(".pdf");
        Assert.That(result, Is.EqualTo(MediaFileType.PDF));
    }

    [Test]
    public void ConvertFileGoodCaseTest()
    {
        Mock<IFormFile> fileStub = new();
        fileStub.Setup(x => x.FileName).Returns(".pdf");
        var result = ExtensionToMediaFileType.Convert(fileStub.Object);
        Assert.That(result, Is.EqualTo(MediaFileType.PDF));
    }
    [Test]
    public void ConvertFileWithWrongFileNameTest()
    {
        Mock<IFormFile> fileStub = new();
        fileStub.Setup(x => x.FileName).Returns("asdasdasd");
        Assert.Throws<NotSupportedException>(() => ExtensionToMediaFileType.Convert(fileStub.Object));
    }
}