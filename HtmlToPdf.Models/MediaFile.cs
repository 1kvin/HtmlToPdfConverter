using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HtmlToPdf.Models;

public class MediaFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public MediaFileType Type { get; set; }
    public string Extension { get; set; } = null!;
    public string Name { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string? ContentType { get; set; }
    public DateTime? UploadDate { get; set; } = null!;
}