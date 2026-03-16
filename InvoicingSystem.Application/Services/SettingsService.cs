using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;

namespace InvoicingSystem.Application.Services;

public class SettingsService(ISettingsRepository settingsRepository) : ISettingsService
{
    public async Task<decimal> GetTaxStampAmount(CancellationToken cancellationToken = default)
    {
        var settings = await settingsRepository.GetOrCreateAsync(cancellationToken);
        return settings.TaxStampAmount;
    }

    public Task UpdateTaxStampAmount(decimal amount, CancellationToken cancellationToken = default)
    {
        if (amount < 0)
        {
            throw new InvalidOperationException("Tax stamp cannot be negative.");
        }

        return settingsRepository.UpdateTaxStampAsync(amount, cancellationToken);
    }
}
