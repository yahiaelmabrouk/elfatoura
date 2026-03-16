using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }

    [Required, StringLength(140)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.001, 1000000000)]
    public decimal PriceHT { get; set; }

    [Range(0.07, 0.19)]
    public decimal VATRate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
