namespace HtmlToPdf.BusinessLogic.Exceptions;

public class FileSizeException : Exception
{
    public FileSizeException(string message) : base(message) {}
    
    public FileSizeException() {}
}