using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Application.Models;
using InvoicingSystem.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvoicingSystem.Infrastructure.Services;

public class ExportService(IInvoiceRepository invoiceRepository) : IExportService
{
    static ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<ExportFileResult> ExportInvoicePdf(int invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, includeDetails: true, cancellationToken)
            ?? throw new InvalidOperationException("Invoice not found.");

        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("Invoice").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Invoice Number: {invoice.InvoiceNumber}");
                    col.Item().Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}");
                    col.Item().Text($"Customer: {invoice.Customer?.Name}");
                    col.Item().Text($"Tax Number: {invoice.Customer?.TaxNumber}");
                });

                page.Content().PaddingVertical(15).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Product").Bold();
                            header.Cell().Element(CellStyle).Text("Qty").Bold();
                            header.Cell().Element(CellStyle).Text("Unit HT").Bold();
                            header.Cell().Element(CellStyle).Text("VAT %").Bold();
                            header.Cell().Element(CellStyle).Text("TTC").Bold();
                        });

                        foreach (var line in invoice.Lines)
                        {
                            table.Cell().Element(CellStyle).Text(line.Product?.Name ?? "-");
                            table.Cell().Element(CellStyle).Text(line.Quantity.ToString(CultureInfo.InvariantCulture));
                            table.Cell().Element(CellStyle).Text(line.UnitPriceHT.ToString("N3", CultureInfo.InvariantCulture));
                            table.Cell().Element(CellStyle).Text((line.VATRate * 100m).ToString("N0", CultureInfo.InvariantCulture));
                            table.Cell().Element(CellStyle).Text(line.LineTotalTTC.ToString("N3", CultureInfo.InvariantCulture));
                        }
                    });

                    col.Item().AlignRight().PaddingTop(20).Column(totals =>
                    {
                        totals.Item().Text($"Total HT: {invoice.TotalHT:N3} TND");
                        totals.Item().Text($"Total VAT: {invoice.TotalVAT:N3} TND");
                        totals.Item().Text($"Tax Stamp: {invoice.TaxStampAmount:N3} TND");
                        totals.Item().Text($"Total TTC: {invoice.TotalTTC:N3} TND").Bold().FontSize(13);
                    });
                });
            });
        }).GeneratePdf();

        return new ExportFileResult
        {
            FileName = $"{invoice.InvoiceNumber}.pdf",
            ContentType = "application/pdf",
            Content = bytes
        };
    }

    public async Task<ExportFileResult> ExportInvoicesCsv(CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetAllAsync(includeDetails: true, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("InvoiceNumber,InvoiceDate,Customer,TotalHT,TotalVAT,TaxStamp,TotalTTC");

        foreach (var invoice in invoices)
        {
            sb.AppendLine(string.Join(',',
                Escape(invoice.InvoiceNumber),
                invoice.InvoiceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Escape(invoice.Customer?.Name ?? string.Empty),
                invoice.TotalHT.ToString("0.000", CultureInfo.InvariantCulture),
                invoice.TotalVAT.ToString("0.000", CultureInfo.InvariantCulture),
                invoice.TaxStampAmount.ToString("0.000", CultureInfo.InvariantCulture),
                invoice.TotalTTC.ToString("0.000", CultureInfo.InvariantCulture)));
        }

        return new ExportFileResult
        {
            FileName = $"invoices-{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = "text/csv",
            Content = Encoding.UTF8.GetBytes(sb.ToString())
        };
    }

    public async Task<ExportFileResult> ExportInvoicesExcel(CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetAllAsync(includeDetails: true, cancellationToken);

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Invoices");
        var headers = new[] { "Invoice #", "Date", "Customer", "Total HT", "Total VAT", "Tax Stamp", "Total TTC" };

        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var row = 2;
        foreach (var invoice in invoices)
        {
            sheet.Cell(row, 1).Value = invoice.InvoiceNumber;
            sheet.Cell(row, 2).Value = invoice.InvoiceDate;
            sheet.Cell(row, 3).Value = invoice.Customer?.Name;
            sheet.Cell(row, 4).Value = invoice.TotalHT;
            sheet.Cell(row, 5).Value = invoice.TotalVAT;
            sheet.Cell(row, 6).Value = invoice.TaxStampAmount;
            sheet.Cell(row, 7).Value = invoice.TotalTTC;
            row++;
        }

        sheet.Column(2).Style.DateFormat.Format = "yyyy-mm-dd";
        for (var c = 4; c <= 7; c++)
        {
            sheet.Column(c).Style.NumberFormat.Format = "0.000";
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new ExportFileResult
        {
            FileName = $"invoices-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Content = stream.ToArray()
        };
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .PaddingVertical(5)
            .PaddingHorizontal(4)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2);
    }
}
