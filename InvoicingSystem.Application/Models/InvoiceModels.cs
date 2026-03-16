namespace InvoicingSystem.Application.Models;

public class CreateInvoiceRequest
{
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public List<CreateInvoiceLineRequest> Lines { get; set; } = new();
}

public class CreateInvoiceLineRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPriceHT { get; set; }
    public decimal? VATRate { get; set; }
}

public class InvoiceLineCalculationResult
{
    public decimal LineTotalHT { get; set; }
    public decimal LineVAT { get; set; }
    public decimal LineTotalTTC { get; set; }
}

public class InvoiceTotals
{
    public decimal TotalHT { get; set; }
    public decimal TotalVAT { get; set; }
    public decimal TaxStampAmount { get; set; }
    public decimal TotalTTC { get; set; }
}
