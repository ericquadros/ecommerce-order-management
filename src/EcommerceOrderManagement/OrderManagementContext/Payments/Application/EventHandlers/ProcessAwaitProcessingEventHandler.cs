using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.Infrastructure.Interfaces;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

public class ProcessAwaitProcessingEventHandler
{
    private readonly OrderManagementDbContext _context;
    private readonly IMessageBroker _brokerService;

    public ProcessAwaitProcessingEventHandler(
        OrderManagementDbContext context, 
        IMessageBroker brokerService)
    {
        _context = context;
        _brokerService = brokerService;
    }

    public async Task<Result<Order>> HandleAsync(OrderCompletedEvent orderCompletedEvent)
    {
        var order = orderCompletedEvent?.Object;
        if (order is null)
            return Result.Failure("Order cannot be null");

        foreach (var item in order.Items)
        {
            var productId = _context.OrderItems
                                    .AsNoTracking()
                                    .FirstOrDefault(i => i.Id == item.Id).ProductId;
            item.AssignItemToProduct(productId);
        }

        order.SetStatusProcessingPayment();

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        
        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await _brokerService.ProduceMessageAsync(domainEvent);

        return order;
    }
}