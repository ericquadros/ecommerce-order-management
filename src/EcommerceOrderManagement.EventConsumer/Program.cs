using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;
using EcommerceOrderManagement.EventConsumer.Infrastructure;
using EcommerceOrderManagement.EventConsumer.Infrastructure.Interfaces;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure;
using EcommerceOrderManagement.EventConsumer.OrderManagementContext.Orders.Infrastructure.EventConsumers;
using EcommerceOrderManagement.Infrastructure.EFContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddJsonFile("appsettings.OrderManagement.json", true, true)
            .AddJsonFile("appsettings.Payment.json", true, true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) => services.ConfigureServices(configuration))
                .Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>();

        if (kafkaSettings == null)
            throw new InvalidOperationException(
                "Kafka settings could not be loaded from configuration file appsettings.");

        var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();
        logger.LogInformation($"Loaded Kafka Settings: {string.Join(", ", kafkaSettings.Topics)}");
        logger.LogInformation($"BootstrapServers: {kafkaSettings.BootstrapServers}");
        logger.LogInformation($"ConsumerGroupId: {kafkaSettings.ConsumerGroupId}");
        logger.LogInformation($"AutoOffsetReset: {kafkaSettings.AutoOffsetReset}");

        services.AddSingleton(kafkaSettings);
        services.AddSingleton<IKafkaMessageProcessorFactory, KafkaMessageProcessorFactory>();
        
        var connectionString = configuration.GetConnectionString("OrderManagementDatabase");
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.ConfigureDomainDependenciesServices();
        // Register your services here
        services.AddSingleton<IKafkaConsumer, KafkaConsumer>();
        services.AddHostedService<KafkaConsumerService>();

        // Register specific processors
        // services.AddScoped<IKafkaEventProcessorStrategy, ProcessPaymentProcessorStrategy>();
        // services.AddScoped<IKafkaEventProcessorStrategy, OrderEventProcessorStrategy>();
        services.AddScoped<IKafkaEventProcessorStrategy, ProcessWaitingProcessingStrategy>();
        // services.AddScoped<IKafkaEventProcessorStrategy, ProcessPaymentProcessorStrategy>(); // TODO: Uncomment
    }
}
