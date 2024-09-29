using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

public class ProcessPaymentDoneEventHandler
{
    private readonly OrderRepository _orderRepository;
    private readonly ILogger<ProcessPaymentDoneEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;

    public ProcessPaymentDoneEventHandler(
        OrderRepository orderRepository,
        ILogger<ProcessPaymentDoneEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task<Result<Order>> HandleAsync(OrderPaymentDoneStatusChangedEvent orderEvent)
    {
        if (orderEvent?.Object?.Id is null)
            return Result.Failure("The order is not setted in the event.");

        var order = await _orderRepository.GetOrderCompleteAsync(orderEvent?.Object?.Id);
        
        var result = order.PickingOrder();
        if (result.IsFailure)
            return null;

        await _orderRepository.UpdateOrderAsync(order);
        
        // Publishing domain events
        foreach (var domainEvent in order.Events)
            await _messageBroker.ProduceMessageAsync(domainEvent);

        return order;
    }
}