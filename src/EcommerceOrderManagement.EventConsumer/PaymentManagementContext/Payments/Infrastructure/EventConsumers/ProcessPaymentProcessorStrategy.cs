using System.Text.Json;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class ProcessPaymentProcessorStrategy : IKafkaEventProcessorStrategy
{
    private const string TOPIC = "eccomerce.order-management-context.order-processing-paymet-status-changed";
    private readonly AjustarPaymentProcessingEventHandler _handler;

    public ProcessPaymentProcessorStrategy(AjustarPaymentProcessingEventHandler handler)
    {
        _handler = handler;
    }

    public bool CanExecute(string topic)
    {
        return topic == TOPIC; 
    }

    public async Task ProcessAsync(string orderCompletedEvent)
    {
        // var orderCompleted = JsonSerializer.Deserialize<OrderCompletedEvent>(orderCompletedEvent);
        //
        // await _handler.HandleAsync(orderCompleted);
        //
        // Console.WriteLine("ProcessPaymentProcessorStrategy - Processing Order: " + orderCompleted.Object.Customer.FirstName);
        // await Task.CompletedTask;
    }
}