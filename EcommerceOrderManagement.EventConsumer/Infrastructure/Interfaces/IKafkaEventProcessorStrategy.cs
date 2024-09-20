namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public interface IKafkaEventProcessorStrategy
{
    bool CanExecute(string topic);
    Task ProcessAsync(string message);
}