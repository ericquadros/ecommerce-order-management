using Confluent.Kafka;

namespace EcommerceOrderManagement.EventConsumer.Infrastructure;

public record KafkaSettings
{
    public List<string> Topics { get; init; } = new();
    public string BootstrapServers { get; init; } = string.Empty;
    public string ConsumerGroupId { get; init; } = string.Empty;
    public AutoOffsetReset AutoOffsetReset { get; init; } = AutoOffsetReset.Earliest;
    public int PartitionsNumbers { get; init; } = 1;
    public short ReplicationFactor { get; init; } = 1;
}