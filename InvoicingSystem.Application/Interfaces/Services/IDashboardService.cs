using InvoicingSystem.Application.Models;

namespace InvoicingSystem.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<TaxDashboardResult> GetTaxDashboard(CancellationToken cancellationToken = default);
    Task<SalesDashboardResult> GetSalesDashboard(PeriodGrouping periodGrouping, CancellationToken cancellationToken = default);
}
