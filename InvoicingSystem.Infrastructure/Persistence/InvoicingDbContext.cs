using InvoicingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Infrastructure.Persistence;

public class InvoicingDbContext(DbContextOptions<InvoicingDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.CustomerId);
            entity.HasIndex(x => x.TaxNumber).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(250);
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.TaxNumber).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.ProductId);
            entity.Property(x => x.Name).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.PriceHT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.VATRate).HasColumnType("decimal(5,4)");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(x => x.InvoiceId);
            entity.HasIndex(x => x.InvoiceNumber).IsUnique();
            entity.Property(x => x.InvoiceNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.TaxStampAmount).HasColumnType("decimal(18,3)");
            entity.Property(x => x.TotalHT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.TotalVAT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.TotalTTC).HasColumnType("decimal(18,3)");

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceLine>(entity =>
        {
            entity.HasKey(x => x.InvoiceLineId);
            entity.Property(x => x.UnitPriceHT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.VATRate).HasColumnType("decimal(5,4)");
            entity.Property(x => x.LineTotalHT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.LineVAT).HasColumnType("decimal(18,3)");
            entity.Property(x => x.LineTotalTTC).HasColumnType("decimal(18,3)");

            entity.HasOne(x => x.Invoice)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.InvoiceLines)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(x => x.SettingId);
            entity.Property(x => x.TaxStampAmount).HasColumnType("decimal(18,3)");
            entity.HasData(new Setting { SettingId = 1, TaxStampAmount = 1m });
        });
    }
}
