using EcommerceOrderManagement.Domain.OrderManagementContext.StockItems.EventHandlers;
using EcommerceOrderManagement.Domain.PaymentManagementContext.StockItems.Application.Events;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.EventConsumer.PaymentManagementContext.Infrastructure.EventConsumers;

public class ProcessPickingOrdersStrategy : IKafkaEventProcessorStrategy
{
    private readonly ProcessPickingItemsOrderEventHandler _handler;
    
    private readonly ILogger<ProcessPaymentDoneStrategy> _logger;
    private const string TOPIC = "e-order-management.order-picking-order-status-changed";
    
    public ProcessPickingOrdersStrategy(
        ProcessPickingItemsOrderEventHandler handler,
        ILogger<ProcessPaymentDoneStrategy> logger)
    {
        _handler = handler;
        _logger = logger;
    }
    
    public bool CanExecute(string topic) => topic == TOPIC; 
    
    public async Task ProcessAsync(string message)
    {
        var orderEvent = JsonConvert.DeserializeObject<OrderPickingItemsStatusChangedEvent>(message);
        _logger.LogInformation($"ProcessPickingOrdersStrategy - Processing - Customer.FirstName: {orderEvent.Object.Customer.FirstName}");
        _logger.LogInformation($"ProcessPickingOrdersStrategy - Processing - Order.Status Actual: {orderEvent.Object.Status}");

        var result = await _handler.HandleAsync(orderEvent);
        
        _logger.LogInformation($"ProcessPickingOrdersStrategy - Processing - Order.Status Changed: {result.Value.Status}");
        LogIsSuccessfullyProcessed(result.IsSuccess);

        await Task.CompletedTask;
    }
    
    private void LogIsSuccessfullyProcessed(bool isSuccess)
    {
        if (isSuccess)
        {
            _logger.LogInformation($"ProcessPickingOrdersStrategy - Executed successfully!!");
            return;
        }
        _logger.LogError($"ProcessPickingOrdersStrategy - Failed to executing processing!!");
    }
}

