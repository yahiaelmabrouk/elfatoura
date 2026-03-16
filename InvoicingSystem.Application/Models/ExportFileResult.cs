namespace InvoicingSystem.Application.Models;

public class ExportFileResult
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}
