using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Sample.API.Shared.API;
using Sample.API.Shared.Data;
using Sample.API.Tests.Shared;
using Shouldly;

namespace Sample.API.Tests.Companies;

[TestFixture]
public class WhenCreatingCompanies : IntegrationTestFixture
{
    [Test]
    public async Task ShouldCreateCompany()
    {
        // Arrange
        var client = AppFactory.GetFlurlClient()
            .WithHeader(VersioningExtensions.ApiVersionHeaderName, "1.0");

        var createCompanyRequest = CompanyFaker.CreateCompanyRequest().Generate();

        // Act
        var createResponse = await client.Request("companies")
            .AllowAnyHttpStatus()
            .PostJsonAsync(createCompanyRequest).ConfigureAwait(false);

        // Assert
        createResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var assertContext = GetRequiredService<ApplicationDbContext>();

        var actualCompany = await assertContext.Companies.FirstOrDefaultAsync(c => c.Name == createCompanyRequest.Name).ConfigureAwait(false);
        actualCompany.ShouldNotBeNull();
    }
}
