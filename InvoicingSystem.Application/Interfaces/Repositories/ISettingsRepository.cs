using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Interfaces.Repositories;

public interface ISettingsRepository
{
    Task<Setting> GetOrCreateAsync(CancellationToken cancellationToken = default);
    Task UpdateTaxStampAsync(decimal amount, CancellationToken cancellationToken = default);
}
