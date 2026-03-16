using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomers(CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerById(int id, CancellationToken cancellationToken = default);
    Task AddCustomer(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateCustomer(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteCustomer(int id, CancellationToken cancellationToken = default);
}
