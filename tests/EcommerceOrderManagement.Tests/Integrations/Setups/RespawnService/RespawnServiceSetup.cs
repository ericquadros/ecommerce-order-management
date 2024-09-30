using Respawn;

namespace EcommerceOrderManagement.Tests.Integrations.RespawnService;

public class RespawnServiceSetup
{
    private static readonly RespawnerOptions Options = new() { TablesToIgnore = ["__EFMigrationsHistory"] };

    public static Respawner Inicializa()
    {
        var respawner = Respawner.CreateAsync(
            IntegrationTestsSetup.Database.ConnectionString,
            Options
        ).GetAwaiter().GetResult();

        return respawner;
    }
}