using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Domain.Entities;
using InvoicingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Infrastructure.Repositories;

public class InvoiceRepository(InvoicingDbContext context) : IInvoiceRepository
{
    public Task<List<Invoice>> GetAllAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = context.Invoices.AsNoTracking();

        if (includeDetails)
        {
            query = query
                .Include(x => x.Customer)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Product);
        }

        return query
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public Task<Invoice?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = context.Invoices;

        if (includeDetails)
        {
            query = query
                .Include(x => x.Customer)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Product);
        }

        return query.FirstOrDefaultAsync(x => x.InvoiceId == id, cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountByYearAsync(int year, CancellationToken cancellationToken = default) =>
        context.Invoices.CountAsync(x => x.InvoiceDate.Year == year, cancellationToken);
}
