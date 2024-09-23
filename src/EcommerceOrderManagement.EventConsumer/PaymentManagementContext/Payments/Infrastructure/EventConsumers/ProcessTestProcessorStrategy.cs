namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

public class ProcessTestProcessorStrategy : IKafkaEventProcessorStrategy
{
    private const string TOPIC = "eccomerce.order-management-context.order-test"; 

    public bool CanExecute(string topic)
    {
        return topic == TOPIC;
    }

    public async Task ProcessAsync(string message)
    {
        // Lógica de processamento para o tópico de pagamento
        Console.WriteLine("ProcessWaitingPayment TEST - Processing Order: " + message);

        // Aqui você pode adicionar a lógica para alterar o estado do pedido
        // e implementar a estratégia de pagamento conforme os dados do pedido.
        await Task.CompletedTask;
    }
}