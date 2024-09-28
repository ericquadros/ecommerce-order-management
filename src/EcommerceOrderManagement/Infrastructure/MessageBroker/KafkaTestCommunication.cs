using Confluent.Kafka;

namespace EcommerceOrderManagement.Infrastructure;

public static class KafkaTestCommunication
{
    public static void Execute(string appName = "default")
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            producer.Produce("test-topic", new Message<Null, string> { Value = $"App: {appName} testing communication successfully." });
            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"Mensagem produzida ");
        }

        var consumerConfig = new ConsumerConfig
        {
            GroupId = $"test-consumer-group-{appName}",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe("test-topic");

            var consumeResult = consumer.Consume();
            Console.WriteLine($"Mensagem recebida: {consumeResult.Message.Value}");
        }
    }
}