using Microsoft.Extensions.DependencyInjection;

namespace Sample.API.Tests.Shared;

public class IntegrationTestApplicationFactory : IntegrationTestWebApplicationFactory<Program>
{
    public IntegrationTestApplicationFactory() : base(IntegrationTestFixture.SetupConfigurationBuilder)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
    }
}
