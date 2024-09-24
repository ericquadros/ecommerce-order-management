using Confluent.Kafka.Admin;
using EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.EventConsumer.Infrastructure;

using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly IEnumerable<IKafkaEventProcessorStrategy> _strategies;
    private readonly KafkaSettings _settings;
    private readonly ILogger _logger;

    public KafkaConsumer(KafkaSettings settings, ILogger<KafkaConsumer> logger, IEnumerable<IKafkaEventProcessorStrategy> strategies)
    {
        _settings = settings;
        _logger = logger;
        _strategies = strategies;
    }

    public async Task ConsumeAsync(string topic, Func<string, Task> processMessage)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = _settings.AutoOffsetReset
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(topic);
            try
            {
                while (true)
                {
                    var cr = consumer.Consume(CancellationToken.None);
                    if (cr.Message != null && !string.IsNullOrWhiteSpace(cr.Topic))
                    {
                        await processMessage(cr.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Erro ao consumir o tópico {topic}: {ex.Message}");
            }
        }
    }

    public async Task ConsumeAsync(IEnumerable<string> topics)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = _settings.AutoOffsetReset
        };
        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(topics);

            try
            {
                while (true)
                {
                    var cr = consumer.Consume(CancellationToken.None);
                    if (cr.Message != null && !string.IsNullOrWhiteSpace(cr.Topic))
                    {
                        await ProcessMessageAsync(cr.Topic, cr.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Erro ao consumir o tópico: {ex.Message}");
            }
        }
    }

    private async Task ProcessMessageAsync(string topic, string message)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.CanExecute(topic))
            {
                await strategy.ProcessAsync(message);
                break; // Para ao encontrar a estratégia correspondente
            }
        }
    }
}

