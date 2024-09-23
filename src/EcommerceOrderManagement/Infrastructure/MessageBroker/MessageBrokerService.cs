using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using EcommerceOrderManagement.Domain.Infrastructure.Interfaces;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.Infrastructure;

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
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
        
            // Obtendo o nome do tópico a partir do EventName usando o enum
            if (Enum.TryParse<EventTypes>(domainEvent.EventName, out var eventType))
            {
                var topic = GetEventTopic(eventType);

                KafkaHelper.EnsureTopicExistsAsync(topic, _configuration, _logger);
        
                var producer = new ProducerBuilder<Null, string>(config).Build();

                var domainEventJson = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = false // Opcional, se você não precisar da indentação para leitura fácil.
                });
                await producer.ProduceAsync(topic, new Message<Null, string> 
                { 
                    Value = domainEventJson 
                });
                await producer.ProduceAsync( "eccomerce.order-management-context.order-test" , new Message<Null, string> 
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