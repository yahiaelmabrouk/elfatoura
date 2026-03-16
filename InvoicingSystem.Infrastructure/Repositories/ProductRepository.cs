using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Domain.Entities;
using InvoicingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Infrastructure.Repositories;

public class ProductRepository(InvoicingDbContext context) : IProductRepository
{
    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Products.FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
