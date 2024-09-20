namespace EcommerceOrderManagement.EventConsumer.Infrastructure;

public record KafkaSettings
{
    public List<string> Topics { get; init; } = new();
    public string BootstrapServers { get; init; } = string.Empty;
    public string ConsumerGroupId { get; init; } = string.Empty;
    public string AutoOffsetReset { get; init; } = "Earliest";
    public int NumPartitions { get; init; } = 1;
    public short ReplicationFactor { get; init; } = 1;
}