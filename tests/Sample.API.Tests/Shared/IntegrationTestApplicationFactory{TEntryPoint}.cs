using System;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.API.Tests.Shared;

public abstract class IntegrationTestWebApplicationFactory<TEntryPoint>(
    Func<IConfigurationBuilder, IConfigurationBuilder> configure)
    : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public virtual FlurlClient GetFlurlClient() =>
        new(CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }));

    public abstract void ConfigureServices(IServiceCollection services);

    public T GetRequiredService<T>()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration(config => configure(config));

        builder.ConfigureServices(
            services =>
            {
                ConfigureServices(services);
                services.BuildServiceProvider();
            });
    }
}
