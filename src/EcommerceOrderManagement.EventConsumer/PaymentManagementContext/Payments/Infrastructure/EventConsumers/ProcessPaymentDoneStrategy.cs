using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.Events;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.EventConsumer.PaymentManagementContext.Infrastructure.EventConsumers;

public class ProcessPaymentDoneStrategy : IKafkaEventProcessorStrategy
{
    private readonly ProcessPaymentDoneEventHandler _handler; // payment done implement
    
    private readonly ILogger<ProcessPaymentDoneStrategy> _logger;
    private const string TOPIC = "ecommerce-order-management.order-payment-done-status-changed";
    
    public ProcessPaymentDoneStrategy(
        ProcessPaymentDoneEventHandler handler,
        ILogger<ProcessPaymentDoneStrategy> logger)
    {
        _handler = handler;
        _logger = logger;
    }
    
    public bool CanExecute(string topic) => topic == TOPIC; 
    
    public async Task ProcessAsync(string message)
    {
        var orderEvent = JsonConvert.DeserializeObject<OrderPaymentDoneStatusChangedEvent>(message);
        var result = await _handler.HandleAsync(orderEvent);
        
        _logger.LogInformation($"ProcessPaymentDoneStrategy - Processing - Customer.FirstName: {orderEvent.Object.Customer.FirstName}");
        _logger.LogInformation($"ProcessPaymentDoneStrategy - Processing - Order.Status: {orderEvent.Object.Status}");
        
        if (result.IsSuccess)
            _logger.LogInformation("ProcessPaymentDoneStrategy - Executed successfully!!");
        else 
            _logger.LogError("ProcessPaymentDoneStrategy - Failed to executing processing!!");
        
        await Task.CompletedTask;
    }
}

