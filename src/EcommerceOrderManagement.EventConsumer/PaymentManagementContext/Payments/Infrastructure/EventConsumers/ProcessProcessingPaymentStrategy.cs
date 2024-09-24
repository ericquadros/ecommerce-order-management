using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.EventConsumer.PaymentManagementContext.Infrastructure.EventConsumers;

public class ProcessProcessingPaymentStrategy : IKafkaEventProcessorStrategy
{
    // private readonly ProcessProcessingPaymentEventHandler _handler;
    //
    private readonly ILogger<ProcessWaitingProcessingStrategy> _logger;
    private const string TOPIC = "eccomerce-order-management.order-processing-payment-status-changed";
    //
    // public ProcessProcessingPaymentStrategy(
    //     ProcessAwaitProcessingEventHandler handler,
    //     ILogger<ProcessWaitingProcessingStrategy> logger)
    // {
    //     _handler = handler;
    //     _logger = logger;
    // }
    //
    public bool CanExecute(string topic) => topic == TOPIC; 
    
    public async Task ProcessAsync(string message)
    {
        // var orderCompletedEvent = JsonConvert.DeserializeObject<OrderCompletedEvent>(message);
        //
        // var result = await _handler.HandleAsync(orderCompletedEvent);
        //
        // _logger.LogInformation($"ProcessPaymentProcessorStrategy - Processing - Customer.FirstName: {orderCompletedEvent.Object.Customer.FirstName}");
        // _logger.LogInformation($"ProcessPaymentProcessorStrategy - Processing - Order.Status: {orderCompletedEvent.Object.Status}");
        //
        // if (result.IsFailure)
        //     _logger.LogError("Failed to executing processing!!");
        
        await Task.CompletedTask;
    }
}

