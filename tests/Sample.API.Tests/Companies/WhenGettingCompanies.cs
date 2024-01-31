using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using NUnit.Framework;
using Sample.API.Companies.Models;
using Sample.API.Shared.API;
using Sample.API.Shared.Data;
using Sample.API.Tests.Shared;
using Shouldly;

namespace Sample.API.Tests.Companies;

[TestFixture]
public class WhenGettingCompanies : IntegrationTestFixture
{
    [Test]
    public async Task ShouldReturnAllCompanies()
    {
        // Arrange
        var arrangeContext = GetRequiredService<ApplicationDbContext>();

        var expectedCompany = CompanyFaker.Company().Generate();

        arrangeContext.Companies.Add(expectedCompany);
        await arrangeContext.SaveChangesAsync().ConfigureAwait(false);

        var client = AppFactory.GetFlurlClient()
            .WithHeader(VersioningExtensions.ApiVersionHeaderName, "1.0");

        // Act
        var models = await client.Request("companies")
            .AllowAnyHttpStatus()
            .GetJsonAsync<IEnumerable<CompanyModel>>().ConfigureAwait(false);

        // Assert
        models.ShouldNotBeNull();
        models.ShouldNotBeEmpty();

        var actualCompany = models.First(x => x.Id == expectedCompany.Id);
        actualCompany.Name.ShouldBe(expectedCompany.Name);
    }
}
