using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

public class ProcessPaymentDoneEventHandler
{
    private readonly OrderManagementDbContext _context;
    private readonly ILogger<ProcessPaymentDoneEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;

    public ProcessPaymentDoneEventHandler(
        OrderManagementDbContext context,
        ILogger<ProcessPaymentDoneEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _context = context;
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task<Result<Order>> HandleAsync(OrderPaymentDoneStatusChangedEvent orderEvent)
    {
        var order = orderEvent.Object;
        
        var result = order.SetStatusPickingOrder();
        if (result.IsFailure)
            return null;
        
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        
        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await _messageBroker.ProduceMessageAsync(domainEvent);

        return order;
    }
}