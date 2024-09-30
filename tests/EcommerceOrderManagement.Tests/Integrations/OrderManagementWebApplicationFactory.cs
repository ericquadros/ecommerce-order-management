using DotNet.Testcontainers.Builders;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NSubstitute;
using Testcontainers.MsSql;

namespace EcommerceOrderManagement.Tests.Integrations;

public class OrderManagementWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = default!;
    
    public OrderManagementWebApplicationFactory()
    {
        var msSqlPassword = "D3vP@ss!8";
        
        var random = new Random();
        var msSqlPort = random.Next(10000, 20001);
        
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName($"integracao-tests-ordermanagement-port-{msSqlPort}")
            .WithPortBinding(msSqlPort, 1433)
            .WithPassword(msSqlPassword)
            .WithEnvironment("SA_PASSWORD", msSqlPassword)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("SQL Server is now ready for client connections"))
            .Build();
    }

    private readonly MockacoContainer _httpMockContainer = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var messageBroker = Substitute.For<IMessageBroker>();

            services.AddScoped(_ => messageBroker);
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _httpMockContainer.InitializeAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings:Database", _sqlContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ExchangeService:ApiUrl", _httpMockContainer.ContainerUri);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _sqlContainer.StopAsync();
        await _httpMockContainer.DisposeAsync();
    }
}