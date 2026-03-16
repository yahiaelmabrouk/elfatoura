using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllAsync(bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<int> CountByYearAsync(int year, CancellationToken cancellationToken = default);
}
