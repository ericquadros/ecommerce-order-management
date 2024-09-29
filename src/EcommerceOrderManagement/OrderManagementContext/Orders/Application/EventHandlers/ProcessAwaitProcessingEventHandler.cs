using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;

namespace EcommerceOrderManagement.PaymentManagementContext.Orders.Application.EventHandlers;

public class ProcessAwaitProcessingEventHandler
{
    private readonly OrderRepository _orderRepository;
    private readonly IMessageBroker _brokerService;

    public ProcessAwaitProcessingEventHandler(
        OrderRepository orderRepository,
        IMessageBroker brokerService)
    {
        _orderRepository = orderRepository;
        _brokerService = brokerService;
    }

    public async Task<Result<Order>> HandleAsync(OrderCompletedEvent orderEvent)
    {
        try
        {
            if (orderEvent?.Object?.Id is null)
                return Result.Failure("The order is not setted in the event.");

            var order = await _orderRepository.GetOrderCompleteAsync(orderEvent?.Object?.Id);

            order.ProcessingPayment();

            await _orderRepository.UpdateOrderAsync(order);
        
            // Publishing domain events
            foreach (var domainEvent in order.Events)
                await _brokerService.ProduceMessageAsync(domainEvent);

            return order;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}