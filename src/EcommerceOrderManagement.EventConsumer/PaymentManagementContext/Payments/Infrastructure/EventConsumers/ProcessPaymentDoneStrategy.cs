using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using EcommerceOrderManagement.PaymentManagementContext.Payments.Application.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.EventConsumer.PaymentManagementContext.Infrastructure.EventConsumers;

public class ProcessPaymentDoneStrategy : IKafkaEventProcessorStrategy
{
    private readonly ProcessProcessingPaymentEventHandler _handler; // payment done implement
    
    private readonly ILogger<ProcessWaitingProcessingStrategy> _logger;
    private const string TOPIC = "ecommerce-order-management.order-payment-done-status-changed";
    
    public ProcessPaymentDoneStrategy(
        ProcessProcessingPaymentEventHandler handler,
        ILogger<ProcessWaitingProcessingStrategy> logger)
    {
        _handler = handler;
        _logger = logger;
    }
    
    public bool CanExecute(string topic) => topic == TOPIC; 
    
    public async Task ProcessAsync(string message)
    {
        var orderEvent = JsonConvert.DeserializeObject<OrderProcessingPaymentStatusChangedEvent>(message);
        var result = await _handler.HandleAsync(orderEvent);
        
        _logger.LogInformation($"ProcessProcessingPaymentStrategy - Processing - Customer.FirstName: {orderEvent.Object.Customer.FirstName}");
        _logger.LogInformation($"ProcessProcessingPaymentStrategy - Processing - Order.Status: {orderEvent.Object.Status}");
        
        if (result.IsFailure)
            _logger.LogError("Failed to executing processing!!");
        
        await Task.CompletedTask;
    }
}

