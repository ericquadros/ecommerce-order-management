using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.Migrations.Context;
using EcommerceOrderManagement.Tests.Integrations.Setups.DatabaseService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace EcommerceOrderManagement.Tests.Integrations;

public class IntegrationTestsSetup : WebApplicationFactory<WebApiProgram>, IAsyncLifetime
{
    public static DatabaseContainerSetup Database { get; private set; } = new();
    
    private readonly MockacoContainerSetup _httpMockContainerSetup = new();
    
    public HttpClient HttpClient { get; private set; } = default!;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var messageBroker = Substitute.For<IMessageBroker>();
            services.AddScoped(_ => messageBroker);
            
            services.AddDbContext<OrderManagementMigrationsDbContext>(options =>
                options.UseSqlServer(Database.ConnectionString));
        });
        
        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await Database.StartDbServer();
        await _httpMockContainerSetup.InitializeAsync();
        
        HttpClient = CreateClient();
        
        Environment.SetEnvironmentVariable("EcommerceOrderMmanagementDatabase", Database.ConnectionString);
        Environment.SetEnvironmentVariable("ExchangeService:ApiUrl", _httpMockContainerSetup.ContainerUri);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Database.StopDbServer();
        await _httpMockContainerSetup.DisposeAsync();
    }
}