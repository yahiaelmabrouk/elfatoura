namespace InvoicingSystem.Domain.Entities;

public class Setting
{
    public int SettingId { get; set; }

    public decimal TaxStampAmount { get; set; } = 1m;
}
