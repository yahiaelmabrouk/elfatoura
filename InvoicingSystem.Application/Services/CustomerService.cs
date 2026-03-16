using InvoicingSystem.Application.Interfaces.Repositories;
using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public Task<List<Customer>> GetAllCustomers(CancellationToken cancellationToken = default) =>
        customerRepository.GetAllAsync(cancellationToken);

    public Task<Customer?> GetCustomerById(int id, CancellationToken cancellationToken = default) =>
        customerRepository.GetByIdAsync(id, cancellationToken);

    public async Task AddCustomer(Customer customer, CancellationToken cancellationToken = default)
    {
        customer.CreatedDate = DateTime.UtcNow;
        await customerRepository.AddAsync(customer, cancellationToken);
    }

    public Task UpdateCustomer(Customer customer, CancellationToken cancellationToken = default) =>
        customerRepository.UpdateAsync(customer, cancellationToken);

    public async Task DeleteCustomer(int id, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return;
        }

        await customerRepository.DeleteAsync(customer, cancellationToken);
    }
}
