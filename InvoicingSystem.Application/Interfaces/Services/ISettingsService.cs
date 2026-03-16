namespace InvoicingSystem.Application.Interfaces.Services;

public interface ISettingsService
{
    Task<decimal> GetTaxStampAmount(CancellationToken cancellationToken = default);
    Task UpdateTaxStampAmount(decimal amount, CancellationToken cancellationToken = default);
}
