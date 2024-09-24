using Confluent.Kafka;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
            if (domainEvent is null)
                return;
            
            var bootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers");
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
        
            // Obtendo o nome do tópico a partir do EventName usando o enum
            if (Enum.TryParse<EventTypes>(domainEvent.EventName, out var eventType))
            {
                // Get topic name from json appsettings
                var topic = GetEventTopic(eventType);

                KafkaHelper.EnsureTopicExistsAsync(topic, _configuration, _logger);
        
                var producer = new ProducerBuilder<Null, string>(config).Build();
                
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignorar referências circulares
                    NullValueHandling = NullValueHandling.Ignore // Ignorar valores nulos
                };
                // Serializa o evento, evitando as referências e eventos
                var domainEventJson = JsonConvert.SerializeObject(domainEvent, jsonSettings);

                await producer.ProduceAsync(topic, new Message<Null, string> 
                { 
                    Value = domainEventJson 
                });
            }
            else
                throw new ArgumentException($"Event type {domainEvent.EventName} is not recognized.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
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