using Confluent.Kafka.Admin;
using EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.EventConsumer.Infrastructure;

using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly KafkaSettings _settings;
    private readonly ILogger _logger;

    public KafkaConsumer(KafkaSettings settings, ILogger<KafkaConsumer> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task ConsumeAsync(string topic, Func<string, Task> processMessage)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = Enum.TryParse<AutoOffsetReset>(_settings.AutoOffsetReset, out var autoOffsetReset) 
                ? autoOffsetReset 
                : AutoOffsetReset.Earliest // fallback to Earliest if the conversion fails
        };
        
        // Create topic if it not exists
        await EnsureTopicExistsAsync(topic);
        
        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            consumer.Subscribe(topic);

            try
            {
                while (true)
                {
                    var cr = consumer.Consume(CancellationToken.None);
                    if (cr.Message != null)
                    {
                        await processMessage(cr.Message.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // The consumer has canceled, excecute it to finalize correctly
                consumer.Close();
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Erro ao consumir o tópico {topic}: {ex.Message}");
            }
        }
    }

    private async Task EnsureTopicExistsAsync(string topic)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };

        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                if (metadata.Topics.Exists(t => t.Topic == topic))
                    return;

                _logger.LogInformation($"O tópico '{topic}' não existe.");

                // Se o tópico não existir, crie-o
                _logger.LogInformation($"Criando o tópico '{topic}'...");

                var topicSpecification = new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = _settings.NumPartitions,
                    ReplicationFactor = _settings.ReplicationFactor
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpecification });

                _logger.LogInformation($"Tópico '{topic}' criado com sucesso.");
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    _logger.LogError($"Erro ao criar o tópico: {e.Results[0].Error.Reason}");
                    throw;
                }
                else
                {
                    _logger.LogInformation($"Tópico '{topic}' já existia.");
                }
            }
        }
    }
}
