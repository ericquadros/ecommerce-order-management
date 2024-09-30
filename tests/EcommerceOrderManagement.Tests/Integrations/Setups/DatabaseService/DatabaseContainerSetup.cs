using DotNet.Testcontainers.Builders;
using EcommerceOrderManagement.Migrations.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace EcommerceOrderManagement.Tests.Integrations.Setups.DatabaseService;

public class DatabaseContainerSetup
{
    private readonly MsSqlContainer _databaseContainer = default!;
    public string ConnectionString { get => _databaseContainer.GetConnectionString(); }

    public DatabaseContainerSetup()
    {
        var msSqlPassword = "D3vP@ss!8";
        
        var random = new Random();
        var msSqlPort = random.Next(10000, 20001);
        
        _databaseContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithName($"integracao-tests-ordermanagement-port-{msSqlPort}")
            .WithPortBinding(msSqlPort, 1433)
            .WithPassword(msSqlPassword)
            .WithEnvironment("SA_PASSWORD", msSqlPassword)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("SQL Server is now ready for client connections"))
            .Build();
    }
    
    public async static Task ExecuteMigrations(OrderManagementMigrationsDbContext _dbContext)
    {
        await _dbContext.Database.MigrateAsync();
    }
    
    public async Task StartDbServer()
    {
        await _databaseContainer.StartAsync();
        Environment.SetEnvironmentVariable("EcommerceOrderMmanagementDatabase", ConnectionString);
    }
    
    public async Task StopDbServer()
    {
        await _databaseContainer.StopAsync();
    }
}