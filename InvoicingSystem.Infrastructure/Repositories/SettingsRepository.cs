using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Domain.Entities;
using InvoicingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Infrastructure.Repositories;

public class SettingsRepository(InvoicingDbContext context) : ISettingsRepository
{
    public async Task<Setting> GetOrCreateAsync(CancellationToken cancellationToken = default)
    {
        var settings = await context.Settings.FirstOrDefaultAsync(x => x.SettingId == 1, cancellationToken);
        if (settings is not null)
        {
            return settings;
        }

        settings = new Setting { SettingId = 1, TaxStampAmount = 1m };
        context.Settings.Add(settings);
        await context.SaveChangesAsync(cancellationToken);
        return settings;
    }

    public async Task UpdateTaxStampAsync(decimal amount, CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateAsync(cancellationToken);
        settings.TaxStampAmount = amount;
        context.Settings.Update(settings);
        await context.SaveChangesAsync(cancellationToken);
    }
}
