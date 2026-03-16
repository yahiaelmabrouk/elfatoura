using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Application.Models;
using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IProductRepository productRepository,
    ISettingsRepository settingsRepository) : IInvoiceService
{
    public async Task<Invoice> CreateInvoice(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Lines.Count == 0)
        {
            throw new InvalidOperationException("At least one invoice line is required.");
        }

        var taxStamp = (await settingsRepository.GetOrCreateAsync(cancellationToken)).TaxStampAmount;
        var invoice = new Invoice
        {
            CustomerId = request.CustomerId,
            InvoiceDate = request.InvoiceDate,
            TaxStampAmount = taxStamp,
            InvoiceNumber = await GenerateInvoiceNumber(request.InvoiceDate, cancellationToken)
        };

        var computedLines = new List<InvoiceLineCalculationResult>();

        foreach (var line in request.Lines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Product {line.ProductId} not found.");

            var unitPrice = line.UnitPriceHT ?? product.PriceHT;
            var vatRate = line.VATRate ?? product.VATRate;
            var calc = AddInvoiceLine(line.Quantity, unitPrice, vatRate);

            computedLines.Add(calc);
            invoice.Lines.Add(new InvoiceLine
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                UnitPriceHT = unitPrice,
                VATRate = vatRate,
                LineTotalHT = calc.LineTotalHT,
                LineVAT = calc.LineVAT,
                LineTotalTTC = calc.LineTotalTTC
            });
        }

        var totals = CalculateInvoiceTotals(computedLines, taxStamp);
        invoice.TotalHT = totals.TotalHT;
        invoice.TotalVAT = totals.TotalVAT;
        invoice.TotalTTC = totals.TotalTTC;

        await invoiceRepository.AddAsync(invoice, cancellationToken);
        return invoice;
    }

    public InvoiceLineCalculationResult AddInvoiceLine(int quantity, decimal unitPriceHt, decimal vatRate)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var lineHt = quantity * unitPriceHt;
        var lineVat = lineHt * vatRate;
        var lineTtc = lineHt + lineVat;

        return new InvoiceLineCalculationResult
        {
            LineTotalHT = decimal.Round(lineHt, 3),
            LineVAT = decimal.Round(lineVat, 3),
            LineTotalTTC = decimal.Round(lineTtc, 3)
        };
    }

    public InvoiceTotals CalculateInvoiceTotals(IEnumerable<InvoiceLineCalculationResult> lines, decimal taxStampAmount)
    {
        var totalHt = lines.Sum(x => x.LineTotalHT);
        var totalVat = lines.Sum(x => x.LineVAT);
        var totalTtc = totalHt + totalVat + taxStampAmount;

        return new InvoiceTotals
        {
            TotalHT = decimal.Round(totalHt, 3),
            TotalVAT = decimal.Round(totalVat, 3),
            TaxStampAmount = decimal.Round(taxStampAmount, 3),
            TotalTTC = decimal.Round(totalTtc, 3)
        };
    }

    public Task<Invoice?> GetInvoiceById(int id, CancellationToken cancellationToken = default) =>
        invoiceRepository.GetByIdAsync(id, includeDetails: true, cancellationToken);

    public Task<List<Invoice>> GetAllInvoices(CancellationToken cancellationToken = default) =>
        invoiceRepository.GetAllAsync(includeDetails: true, cancellationToken);

    private async Task<string> GenerateInvoiceNumber(DateTime invoiceDate, CancellationToken cancellationToken)
    {
        var year = invoiceDate.Year;
        var countInYear = await invoiceRepository.CountByYearAsync(year, cancellationToken);
        var sequence = countInYear + 1;
        return $"INV-{year}-{sequence:D4}";
    }
}
