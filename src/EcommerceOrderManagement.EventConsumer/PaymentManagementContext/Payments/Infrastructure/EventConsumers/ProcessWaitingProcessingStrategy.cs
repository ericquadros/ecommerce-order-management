using System.Text.Json;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class ProcessWaitingProcessingStrategy : IKafkaEventProcessorStrategy
{
    private readonly ProcessAwaitProcessingEventHandler _handler;
    private const string TOPIC = "eccomerce-order-management.order-created-waiting-processing"; 

    public ProcessWaitingProcessingStrategy(ProcessAwaitProcessingEventHandler handler)
    {
        _handler = handler;
    }
    
    public bool CanExecute(string topic)
    {
        return topic == TOPIC;
    }

    public async Task ProcessAsync(string message)
    {
        var orderCompletedEvent = JsonConvert.DeserializeObject<OrderCompletedEvent>(message);
      
        var result = await _handler.HandleAsync(orderCompletedEvent);
        
        Console.WriteLine("ProcessPaymentProcessorStrategy - Processing - Customer.FirstName: " + orderCompletedEvent.Object.Customer.FirstName);
        Console.WriteLine("ProcessPaymentProcessorStrategy - Processing - Order.Status: " + orderCompletedEvent.Object.Status);
        
        if (result.IsFailure)
            Console.WriteLine("Falha!!!");
        
        await Task.CompletedTask;
    }
    
    private record CardPayment(
        string CardNumber,
        string CardHolder,
        string ExpirationDate,
        string CVV,
        string OrderId,
        string Id,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}

