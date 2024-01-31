using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.API.Tests.Shared;

public abstract class IntegrationTestFixtureBase<TFactory, TStartup>
    where TFactory : IntegrationTestWebApplicationFactory<TStartup> where TStartup : class
{
    public TFactory AppFactory { get; protected set; }

    public IServiceScope ServiceScope { get; private set; }

    public static IConfigurationBuilder SetupConfigurationBuilder(IConfigurationBuilder configurationBuilder) =>
        configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json"), false);

    protected static IConfigurationRoot GetConfigurationRoot() =>
        SetupConfigurationBuilder(new ConfigurationBuilder()).Build();

    protected async Task MigrateDatabaseAsync<TDataContext>() where TDataContext : DbContext
    {
        await using var context = ServiceScope.ServiceProvider.GetService<TDataContext>();
        await context.Database.MigrateAsync();
    }

    public T GetRequiredService<T>() => AppFactory != null ? AppFactory.GetRequiredService<T>() : default;

    public virtual Task Setup()
    {
        if (AppFactory == null)
        {
            return Task.CompletedTask;
        }

        ServiceScope = AppFactory.Services.CreateScope();

        return Task.CompletedTask;
    }

    public virtual void Reset()
    {
        ServiceScope?.Dispose();
        AppFactory?.Dispose();
    }
}
