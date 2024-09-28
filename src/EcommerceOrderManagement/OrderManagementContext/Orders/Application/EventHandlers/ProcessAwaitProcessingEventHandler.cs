using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.PaymentManagementContext.Orders.Application.EventHandlers;

public class ProcessAwaitProcessingEventHandler
{
    private readonly IDbContextFactory _dbContextFactory;
    private readonly IMessageBroker _brokerService;

    public ProcessAwaitProcessingEventHandler(
        IDbContextFactory dbContextFactory, 
        IMessageBroker brokerService)
    {
        _dbContextFactory = dbContextFactory;
        _brokerService = brokerService;
    }

    public async Task<Result<Order>> HandleAsync(OrderCompletedEvent orderCompletedEvent)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var order = orderCompletedEvent?.Object;
        if (order is null)
            return Result.Failure("Order cannot be null");

        foreach (var item in order.Items)
        {
            if (item.ProductId == Guid.Empty) // TODO: REMOVE
            {
                var productId = context.OrderItems
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id == item.Id).ProductId;
                item.AssignItemToProduct(productId);
            }
        }

        order.ProcessingPayment();

        context.Orders.Update(order);
        await context.SaveChangesAsync();
        
        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await _brokerService.ProduceMessageAsync(domainEvent);

        return order;
    }
}