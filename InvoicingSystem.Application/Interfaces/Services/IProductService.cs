using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<Product>> GetProducts(CancellationToken cancellationToken = default);
    Task<Product?> GetProductById(int id, CancellationToken cancellationToken = default);
    Task CreateProduct(Product product, CancellationToken cancellationToken = default);
    Task UpdateProduct(Product product, CancellationToken cancellationToken = default);
    Task DeleteProduct(int id, CancellationToken cancellationToken = default);
}
