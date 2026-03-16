using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.Domain.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }

    [Required, StringLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal TaxStampAmount { get; set; }

    public decimal TotalHT { get; set; }

    public decimal TotalVAT { get; set; }

    public decimal TotalTTC { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}
