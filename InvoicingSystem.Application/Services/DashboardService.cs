using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Application.Models;

namespace InvoicingSystem.Application.Services;

public class DashboardService(IInvoiceRepository invoiceRepository) : IDashboardService
{
    public async Task<TaxDashboardResult> GetTaxDashboard(CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetAllAsync(includeDetails: true, cancellationToken);
        var lines = invoices.SelectMany(i => i.Lines);

        var totalVat = invoices.Sum(i => i.TotalVAT);
        var totalTaxStamp = invoices.Sum(i => i.TaxStampAmount);

        var vatByRate = lines
            .GroupBy(l => l.VATRate)
            .Select(g => new VatByRateItem
            {
                VatRate = g.Key,
                Amount = decimal.Round(g.Sum(x => x.LineVAT), 3)
            })
            .OrderBy(x => x.VatRate)
            .ToList();

        return new TaxDashboardResult
        {
            TotalVatCollected = decimal.Round(totalVat, 3),
            TotalTaxStampAmount = decimal.Round(totalTaxStamp, 3),
            VatByRate = vatByRate
        };
    }

    public async Task<SalesDashboardResult> GetSalesDashboard(PeriodGrouping periodGrouping, CancellationToken cancellationToken = default)
    {
        var invoices = await invoiceRepository.GetAllAsync(includeDetails: true, cancellationToken);

        var revenuePerPeriod = invoices
            .GroupBy(i => BuildPeriodLabel(i.InvoiceDate, periodGrouping))
            .Select(g => new RevenuePeriodItem
            {
                Period = g.Key,
                TotalHT = decimal.Round(g.Sum(x => x.TotalHT), 3),
                TotalTTC = decimal.Round(g.Sum(x => x.TotalTTC), 3)
            })
            .OrderBy(x => x.Period)
            .ToList();

        var revenuePerCustomer = invoices
            .GroupBy(i => i.Customer?.Name ?? "Unknown")
            .Select(g => new RevenueByNameItem
            {
                Name = g.Key,
                Total = decimal.Round(g.Sum(x => x.TotalTTC), 3)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var revenuePerProduct = invoices
            .SelectMany(i => i.Lines)
            .GroupBy(l => l.Product?.Name ?? "Unknown")
            .Select(g => new RevenueByNameItem
            {
                Name = g.Key,
                Total = decimal.Round(g.Sum(x => x.Quantity * x.UnitPriceHT), 3)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        return new SalesDashboardResult
        {
            TotalHT = decimal.Round(invoices.Sum(i => i.TotalHT), 3),
            TotalTTC = decimal.Round(invoices.Sum(i => i.TotalTTC), 3),
            RevenuePerPeriod = revenuePerPeriod,
            RevenuePerCustomer = revenuePerCustomer,
            RevenuePerProduct = revenuePerProduct
        };
    }

    private static string BuildPeriodLabel(DateTime date, PeriodGrouping grouping) => grouping switch
    {
        PeriodGrouping.Day => date.ToString("yyyy-MM-dd"),
        PeriodGrouping.Month => date.ToString("yyyy-MM"),
        PeriodGrouping.Year => date.ToString("yyyy"),
        _ => date.ToString("yyyy-MM")
    };
}
