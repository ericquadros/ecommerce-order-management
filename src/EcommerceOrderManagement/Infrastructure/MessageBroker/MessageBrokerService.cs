using System.Text.Json;
using Confluent.Kafka;
using EcommerceOrderManagement.Domain.Infrastructure.Interfaces;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EcommerceOrderManagement.Domain.Infrastructure;

public class MessageBrokerService : IMessageBroker
{
    private readonly IConfiguration _configuration;
    public MessageBrokerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ProduceMessageAsync<T>(IDomainEvent<T> domainEvent) where T : Entity, IEntity
    {
        var bootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers");
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        
        // Obtendo o nome do tópico a partir do EventName usando o enum
        if (Enum.TryParse<EEventTypes>(domainEvent.EventName, out var eventType))
        {
            var topic = GetEventTopic(eventType);
        
            var producer = new ProducerBuilder<Null, string>(config).Build();

            await producer.ProduceAsync(topic, new Message<Null, string> 
            { 
                Value = JsonSerializer.Serialize(domainEvent) 
            });
        }
        else
        {
            throw new ArgumentException($"Event type {domainEvent.EventName} is not recognized.");
        }
    }
    
    private string GetEventTopic(EEventTypes eventType)
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