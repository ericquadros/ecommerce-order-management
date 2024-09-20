namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class ProcessPaymentProcessorStrategy : IKafkaEventProcessorStrategy
{
    private const string TOPIC = "payment-management:process-payment"; 
    
    public bool CanExecute(string topic)
    {
        return topic == TOPIC; // Verifica se o tópico corresponde a "order-topic"
    }

    public async Task ProcessAsync(string message)
    {
        // Lógica de processamento para o tópico de pedidos
        Console.WriteLine("ProcessPaymentProcessorStrategy - Processing Order: " + message);
        await Task.CompletedTask;
    }
}