using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;
using EcommerceOrderManagement.PaymentManagementContext.Orders.Application.EventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceOrderManagement.Infrastructure;

public static class DependenciesConfigurationExtensions
{
    public static void ConfigureDomainDependenciesServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageBroker, MessageBrokerService>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<GetOrdersQueryHandler>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ProcessAwaitProcessingEventHandler>();
        services.AddScoped<ProcessProcessingPaymentEventHandler>();
        
        // services.AddScoped<IEfDbContextAccessor<DigitacaoDbContextAccessor>>(); // Disabled for now
        
        services.AddScoped<IDiscountStrategy, OrderQuantityDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, OrderSeasonalDiscountStrategy>();
    }
}