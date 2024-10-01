using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.SchedulerWorker.Configuration;
using EcommerceOrderManagement.SchedulerWorker.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;

namespace EcommerceOrderManagement.SchedulerWorker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Console.WriteLine($"environment: {environment}");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddUserSecrets<Program>(optional: true) 
            .AddEnvironmentVariables()
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext() 
            .CreateLogger();

        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureServices(configuration);
                    // services.ConfigureDomainDependenciesServices();
                    services.AddTransient<IOrderRepository, OrderRepository>();
                    services.AddTransient<NotificateOwnerProductsJob>();

                    var cronJobsSettings = context.Configuration.GetSection("CronsJobs").Get<CronsJobsSettings>();
                    
                    services.BuildServiceProvider().GetService<NotificateOwnerProductsJob>().Teste(); // Para testar, descomentar                    
                    
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();

                        // Define o job e o trigger
                        var jobKey = new JobKey("orderJob");
                        q.AddJob<NotificateOwnerProductsJob>(opts => opts.WithIdentity(jobKey));
                        
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity("orderJob-trigger")
                            .WithCronSchedule(cronJobsSettings.NotificateOwnerProductsJob)); // Executa uma vez por dia à meia-noite
                        
                        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                    });

                })
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
        var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();
        
        var connectionString = configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContextFactory<OrderManagementDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.ConfigureDomainDependenciesServices();
        // Register your services here

    }
}