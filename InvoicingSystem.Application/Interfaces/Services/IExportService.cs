using InvoicingSystem.Application.Models;

namespace InvoicingSystem.Application.Interfaces.Services;

public interface IExportService
{
    Task<ExportFileResult> ExportInvoicePdf(int invoiceId, CancellationToken cancellationToken = default);
    Task<ExportFileResult> ExportInvoicesCsv(CancellationToken cancellationToken = default);
    Task<ExportFileResult> ExportInvoicesExcel(CancellationToken cancellationToken = default);
}
