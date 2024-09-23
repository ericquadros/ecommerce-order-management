using EcommerceOrderManagement.EventConsumer.Infrastructure;
using EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;

namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure;

public class KafkaConsumerService : IHostedService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IKafkaMessageProcessorFactory _messageProcessorFactory;

    public KafkaConsumerService(IKafkaConsumer kafkaConsumer,  IKafkaMessageProcessorFactory messageProcessorFactory, KafkaSettings kafkaSettings)
    {
        _kafkaConsumer = kafkaConsumer;
        _kafkaSettings = kafkaSettings;
        _messageProcessorFactory = messageProcessorFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Consume each topic in a separate task to allow for parallel consumption
        var tasks = new List<Task>();

        foreach (var topic in _kafkaSettings.Topics)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _kafkaConsumer.ConsumeAsync(topic, async (message) =>
                {
                    // Select correct processor and process the message
                    var processor = _messageProcessorFactory.GetProcessor(topic);
                    await processor.ProcessAsync(message);
                });
            }, cancellationToken));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}