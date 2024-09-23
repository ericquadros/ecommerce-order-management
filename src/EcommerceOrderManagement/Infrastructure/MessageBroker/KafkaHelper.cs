using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.Infrastructure;

public static class KafkaHelper
{
    public static async Task EnsureTopicExistsAsync(string topic, IConfiguration configuration, ILogger logger)
    {
        var bootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
        var partitions = configuration.GetValue<int>("Kafka:PartitionsNumbers");
        var replicationFactor = configuration.GetValue<short>("Kafka:ReplicationFactor");
        
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                // Retrieve metadata of existing topics
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

                // Check if the topic already exists
                if (metadata.Topics.Exists(t => t.Topic == topic))
                {
                    logger.LogInformation($"The topic '{topic}' already exists.");
                    return;
                }

                logger.LogInformation($"The topic '{topic}' does not exist. Creating the topic '{topic}'...");

                // If the topic does not exist, create it
                var topicSpecification = new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = partitions,
                    ReplicationFactor = replicationFactor
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpecification });

                logger.LogInformation($"Topic '{topic}' created successfully.");
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    logger.LogError($"Error creating the topic '{topic}': {e.Results[0].Error.Reason}");
                    throw;
                }
                else
                {
                    logger.LogInformation($"The topic '{topic}' already existed.");
                }
            }
        }
    }

    public static async Task DeleteTopicAsync(string topic, string bootstrapServers, ILogger logger)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                // Attempt to delete the topic
                await adminClient.DeleteTopicsAsync(new[] { topic });

                logger.LogInformation($"Topic '{topic}' deleted successfully.");
            }
            catch (DeleteTopicsException e)
            {
                logger.LogError($"Error deleting the topic '{topic}': {e.Results[0].Error.Reason}");
                throw;
            }
        }
    }

    public static List<string> GetAllTopics(string bootstrapServers, ILogger logger)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        using (var adminClient = new AdminClientBuilder(adminConfig).Build())
        {
            try
            {
                // Retrieve metadata and return the list of topics
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var topicNames = metadata.Topics.Select(t => t.Topic).ToList();

                logger.LogInformation($"Available topics: {string.Join(", ", topicNames)}");
                return topicNames;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error retrieving the list of topics: {ex.Message}");
                throw;
            }
        }
    }
}