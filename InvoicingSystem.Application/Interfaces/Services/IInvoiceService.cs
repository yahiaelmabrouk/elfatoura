using InvoicingSystem.Application.Models;
using InvoicingSystem.Domain.Entities;

namespace InvoicingSystem.Application.Interfaces.Services;

public interface IInvoiceService
{
    Task<Invoice> CreateInvoice(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    InvoiceLineCalculationResult AddInvoiceLine(int quantity, decimal unitPriceHt, decimal vatRate);
    InvoiceTotals CalculateInvoiceTotals(IEnumerable<InvoiceLineCalculationResult> lines, decimal taxStampAmount);
    Task<Invoice?> GetInvoiceById(int id, CancellationToken cancellationToken = default);
    Task<List<Invoice>> GetAllInvoices(CancellationToken cancellationToken = default);
}
