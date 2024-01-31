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
public class WhenDeletingCompanies : IntegrationTestFixture
{
    [Test]
    public async Task ShouldDeleteCompany()
    {
        // Arrange
        var arrangeContext = GetRequiredService<ApplicationDbContext>();

        var expectedCompany = CompanyFaker.Company().Generate();

        arrangeContext.Companies.Add(expectedCompany);
        await arrangeContext.SaveChangesAsync().ConfigureAwait(false);

        var client = AppFactory.GetFlurlClient()
            .WithHeader(VersioningExtensions.ApiVersionHeaderName, "1.0");

        // Act
        var deleteResponse = await client.Request("companies", expectedCompany.Id)
            .AllowAnyHttpStatus()
            .DeleteAsync().ConfigureAwait(false);

        // Assert
        deleteResponse.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var assertContext = GetRequiredService<ApplicationDbContext>();

        var actualCompany = await assertContext.Companies
            .FirstOrDefaultAsync(c => c.Id == expectedCompany.Id)
            .ConfigureAwait(false);

        actualCompany.ShouldBeNull();
    }
}
