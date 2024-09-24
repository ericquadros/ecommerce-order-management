using System.Text.Json;
using EcommerceOrderManagement.OrderManagementContext.Orders.Events;
using EcommerceOrderManagement.PaymentManagementContext.Orders.Application.EventHandlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class ProcessWaitingProcessingStrategy : IKafkaEventProcessorStrategy
{
    private readonly ProcessAwaitProcessingEventHandler _handler;
    private readonly ILogger<ProcessWaitingProcessingStrategy> _logger;
    private const string TOPIC = "ecommerce-order-management.order-created-waiting-processing"; 

    public ProcessWaitingProcessingStrategy(
        ProcessAwaitProcessingEventHandler handler,
        ILogger<ProcessWaitingProcessingStrategy> logger)
    {
        _handler = handler;
        _logger = logger;
    }
    
    public bool CanExecute(string topic) => topic == TOPIC; 

    public async Task ProcessAsync(string message)
    {
        var orderCompletedEvent = JsonConvert.DeserializeObject<OrderCompletedEvent>(message);
        
        var result = await _handler.HandleAsync(orderCompletedEvent);

        _logger.LogInformation($"ProcessWaitingProcessingStrategy - Processing - Customer.FirstName: {orderCompletedEvent.Object.Customer.FirstName}");
        _logger.LogInformation($"ProcessWaitingProcessingStrategy - Processing - Order.Status: {orderCompletedEvent.Object.Status}");
        
        if (result.IsFailure)
            _logger.LogError("Failed to executing processing!!");
        
        await Task.CompletedTask;
    }
}

