namespace EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;

public interface IKafkaConsumer
{
    Task ConsumeAsync(string topic, Func<string, Task> processMessage);
    Task ConsumeAsync(IEnumerable<string> topics);
}