using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Domain.OrderManagementContext.StockItems.EventHandlers;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.Infrastructure.Mail;
using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;
using EcommerceOrderManagement.OrderManagementContext.StockItems.Domain.Services;
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
        services.AddScoped<OrderRepository>();
        
        services.AddScoped<InventoryService>();
        services.AddScoped<CustomerEmailService>();
        
        services.AddScoped<ProcessAwaitProcessingEventHandler>();
        services.AddScoped<ProcessProcessingPaymentEventHandler>();
        services.AddScoped<ProcessPaymentDoneEventHandler>();
        services.AddScoped<ProcessPickingItemsOrderEventHandler>();
        
        services.AddScoped<IDiscountStrategy, OrderQuantityDiscountStrategy>();
        services.AddScoped<IDiscountStrategy, OrderSeasonalDiscountStrategy>();
    }
}