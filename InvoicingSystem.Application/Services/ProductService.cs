using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private static readonly decimal[] SupportedVatRates = [0.07m, 0.13m, 0.19m];

    public Task<List<Product>> GetProducts(CancellationToken cancellationToken = default) =>
        productRepository.GetAllAsync(cancellationToken);

    public Task<Product?> GetProductById(int id, CancellationToken cancellationToken = default) =>
        productRepository.GetByIdAsync(id, cancellationToken);

    public async Task CreateProduct(Product product, CancellationToken cancellationToken = default)
    {
        ValidateProduct(product);
        product.CreatedDate = DateTime.UtcNow;
        await productRepository.AddAsync(product, cancellationToken);
    }

    public async Task UpdateProduct(Product product, CancellationToken cancellationToken = default)
    {
        ValidateProduct(product);
        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteProduct(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return;
        }

        await productRepository.DeleteAsync(product, cancellationToken);
    }

    private static void ValidateProduct(Product product)
    {
        if (product.PriceHT <= 0)
        {
            throw new InvalidOperationException("Price HT must be positive.");
        }

        if (!SupportedVatRates.Contains(product.VATRate))
        {
            throw new InvalidOperationException("VAT rate is invalid. Allowed values are 7%, 13%, 19%.");
        }
    }
}
