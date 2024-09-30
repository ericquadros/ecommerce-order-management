using EcommerceOrderManagement.Migrations.Context;
using EcommerceOrderManagement.Tests.Integrations.RespawnService;
using EcommerceOrderManagement.Tests.Integrations.Setups;
using EcommerceOrderManagement.Tests.Integrations.Setups.DatabaseService;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace EcommerceOrderManagement.Tests.Integrations;

public abstract class BaseTest : IClassFixture<IntegrationTestsSetup>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly Respawner _checkpoint;
    protected readonly OrderManagementMigrationsDbContext _dbContext;
    
    public BaseTest(IntegrationTestsSetup factory)
    {
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<OrderManagementMigrationsDbContext>();
        _checkpoint = RespawnServiceSetup.Inicializa();

        DatabaseContainerSetup.ExecuteMigrations(_dbContext).GetAwaiter().GetResult();
        DatabaseHelper.SeedInitialDatabase(_dbContext);
    }
    
    public async Task DisposeDatabase()
    {
        await _checkpoint.ResetAsync(IntegrationTestsSetup.Database.ConnectionString);
    }

    void IDisposable.Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }
}