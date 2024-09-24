using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;

namespace EcommerceOrderManagement.EventConsumer.Infrastructure;

public interface IKafkaMessageProcessorFactory
{
    IKafkaEventProcessorStrategy GetProcessor(string topic);
}

public class KafkaMessageProcessorFactory : IKafkaMessageProcessorFactory
{
    private readonly IEnumerable<IKafkaEventProcessorStrategy> _processorStrategies;

    public KafkaMessageProcessorFactory(IEnumerable<IKafkaEventProcessorStrategy> processorStrategies)
    {
        _processorStrategies = processorStrategies;
    }

    public IKafkaEventProcessorStrategy GetProcessor(string topic)
    {
        // Procura na lista de processadores aquele que pode processar o tópico
        var processor = _processorStrategies.FirstOrDefault(p => p.CanExecute(topic));
        
        if (processor == null)
            throw new ArgumentException($"No processor found for this topic: {topic}");

        return processor;
    }
}
