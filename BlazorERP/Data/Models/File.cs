namespace BlazorERP.Data.Models;

public class ErpFile
{
    public int? Id { get; set; }
    public string? FileName { get; set; }
    public string? FileExtension { get; set; }
    public string? MimeType { get; set; }
    public int? Size { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public byte[]? Bytes { get; set; }
}

