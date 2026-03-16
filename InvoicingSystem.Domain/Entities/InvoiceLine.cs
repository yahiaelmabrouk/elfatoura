namespace InvoicingSystem.Domain.Entities;

public class InvoiceLine
{
    public int InvoiceLineId { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPriceHT { get; set; }

    public decimal VATRate { get; set; }

    public decimal LineTotalHT { get; set; }

    public decimal LineVAT { get; set; }

    public decimal LineTotalTTC { get; set; }

    public Invoice? Invoice { get; set; }

    public Product? Product { get; set; }
}
