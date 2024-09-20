namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class OrderEventProcessorStrategy : IKafkaEventProcessorStrategy
{
    public bool CanExecute(string topic)
    {
        return topic == "order-topic"; // Verifica se o tópico corresponde a "order-topic"
    }

    public async Task ProcessAsync(string message)
    {
        // Lógica de processamento para o tópico de pedidos
        Console.WriteLine("Processing Order: " + message);
        await Task.CompletedTask;
    }
}