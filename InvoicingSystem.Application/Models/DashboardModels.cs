namespace InvoicingSystem.Application.Models;

public enum PeriodGrouping
{
    Day,
    Month,
    Year
}

public class VatByRateItem
{
    public decimal VatRate { get; set; }
    public decimal Amount { get; set; }
}

public class RevenuePeriodItem
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalHT { get; set; }
    public decimal TotalTTC { get; set; }
}

public class RevenueByNameItem
{
    public string Name { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class TaxDashboardResult
{
    public decimal TotalVatCollected { get; set; }
    public decimal TotalTaxStampAmount { get; set; }
    public List<VatByRateItem> VatByRate { get; set; } = new();
}

public class SalesDashboardResult
{
    public decimal TotalHT { get; set; }
    public decimal TotalTTC { get; set; }
    public List<RevenuePeriodItem> RevenuePerPeriod { get; set; } = new();
    public List<RevenueByNameItem> RevenuePerCustomer { get; set; } = new();
    public List<RevenueByNameItem> RevenuePerProduct { get; set; } = new();
}
