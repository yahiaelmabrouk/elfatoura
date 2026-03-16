using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Domain.Entities;
using InvoicingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Infrastructure.Repositories;

public class CustomerRepository(InvoicingDbContext context) : ICustomerRepository
{
    public Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Customers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id, cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Update(customer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Remove(customer);
        await context.SaveChangesAsync(cancellationToken);
    }
}
