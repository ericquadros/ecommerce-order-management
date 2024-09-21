using EcommerceOrderManagement.Domain.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceOrderManagement.Domain.Infrastructure;

public static class DependenciesConfigurationExtensions
{
    public static void ConfigureDomainDependenciesServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageBroker, MessageBrokerService>();
        services.AddScoped<CreateOrderHandler>();
        // services.AddScoped<IEfDbContextAccessor<DigitacaoDbContextAccessor>>(); // Disabled for now
    }
}