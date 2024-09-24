using Confluent.Kafka;
using EcommerceOrderManagement.EventConsumer.Infrastructure;
using EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure;

public class KafkaConsumerService : IHostedService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IKafkaMessageProcessorFactory _messageProcessorFactory;

    public KafkaConsumerService(
        IKafkaConsumer kafkaConsumer,
        IKafkaMessageProcessorFactory messageProcessorFactory,
        KafkaSettings kafkaSettings, 
        ILogger<KafkaConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _kafkaSettings = kafkaSettings;
        _logger = logger;
        _messageProcessorFactory = messageProcessorFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Execute multiple consumers in parallel using Task.Run or Parallel.ForEach
        _logger.LogInformation("Starting KafkaConsumerService...");
        
        var tasks = _kafkaSettings.Topics
            .Select(topic => Task.Run(() => StartConsumingTopic(topic, cancellationToken), cancellationToken))
            .ToArray();
        
        // Await completion of all topic consumer tasks
        await Task.WhenAll(tasks);
    }
    
    private async Task StartConsumingTopic(string topic, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting Kafka consumption for topic: {topic}");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _kafkaConsumer.ConsumeAsync(topic, async (message) =>
                {
                    try
                    {
                        var processor = _messageProcessorFactory.GetProcessor(topic);
                        await processor.ProcessAsync(message);
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.LogError($"No processor found for topic {topic}: {ex.Message}");
                        throw;
                    }
                });
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Kafka consumption error on topic {topic}: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken); // Retry delay
            }
        }

        _logger.LogInformation($"Stopped Kafka consumption for topic: {topic}");
    }
    

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}