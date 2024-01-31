using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;
using Sample.API.Shared.Data;

namespace Sample.API.Tests.Shared;

public abstract class IntegrationTestFixture : IntegrationTestFixtureBase<IntegrationTestApplicationFactory, Program>
{
    private IConfigurationRoot configurationRoot;
    private Respawner databaseRespawner;

    [SetUp]
    public virtual async Task SetUp()
    {
        configurationRoot = GetConfigurationRoot();

        AppFactory = new IntegrationTestApplicationFactory();

        await base.Setup();

        var dataContext = GetRequiredService<ApplicationDbContext>();

        databaseRespawner =
            await Respawner.CreateAsync(configurationRoot.GetValue<string>(DataSettings.SqlConnectionStringConfigKey),
                new RespawnerOptions
                {
                    SchemasToInclude = new[] { "dbo" },
                    TablesToIgnore = new[] { new Table("__EFMigrationsHistory") }
                });

        await MigrateDatabaseAsync<ApplicationDbContext>();
    }

    [OneTimeTearDown]
    public virtual void TearDown() => Reset();

    public override void Reset()
    {
        if (databaseRespawner != null)
        {
            var connectionString =
                configurationRoot.GetValue<string>(DataSettings.SqlConnectionStringConfigKey);
            databaseRespawner.ResetAsync(connectionString).Wait();
        }

        base.Reset();
    }
}
