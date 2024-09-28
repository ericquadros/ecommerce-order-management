using Confluent.Kafka;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.Infrastructure;

public class MessageBrokerService : IMessageBroker
{
    private readonly IConfiguration _configuration;
    private readonly  ILogger<MessageBrokerService> _logger;
    public MessageBrokerService(IConfiguration configuration, ILogger<MessageBrokerService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProduceMessageAsync<T>(IDomainEvent<T> domainEvent) where T : Entity, IEntity
    {
        try
        {
            var bootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers");
            var shoudVerifyTopicExists = _configuration.GetValue<bool>("Kafka:ShoudVerifyTopicExists");
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
        
            // Obtendo o nome do tópico a partir do EventName usando o enum
            if (Enum.TryParse<EventTypes>(domainEvent.EventName, out var eventType))
            {
                var topic = GetEventTopic(eventType); // Get topic name from json appsettings
                if (string.IsNullOrEmpty(topic))
                    throw new ArgumentException($"Topic not found for event name: {domainEvent.EventName} in appsettings.");
                
                if (shoudVerifyTopicExists) 
                    KafkaHelper.EnsureTopicExistsAsync(topic, _configuration, _logger);
        
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignorar referências circulares
                    NullValueHandling = NullValueHandling.Ignore // Ignorar valores nulos
                };
                
                var domainEventJson = JsonConvert.SerializeObject(domainEvent, jsonSettings);
                _logger.LogInformation($"Serialized event: {domainEventJson}");

                using var producer = new ProducerBuilder<Null, string>(config).Build();
                await producer.ProduceAsync(topic, new Message<Null, string> 
                { 
                    Value = domainEventJson 
                });
                // producer.Flush(TimeSpan.FromSeconds(10));
            }
            else
                throw new ArgumentException($"Event type {domainEvent.EventName} is not recognized.");
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            throw;
        }
    }
    
    private string GetEventTopic(EventTypes eventType)
    {
        var eventName = eventType.ToString();
        return _configuration.GetValue<string>($"Kafka:EventMapping:{eventName}");
    }
    
    /*
     *   var producer = new ProducerBuilder<Null, string>(config)
                 .Build();

             // await producer.ProduceAsync("finance.control.events", new Message<Null, string> { Value = JsonSerializer.Serialize(message) });
     */
}