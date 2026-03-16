using InvoicingSystem.Application.Interfaces.Services;
using InvoicingSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISettingsService, SettingsService>();

        return services;
    }
}
