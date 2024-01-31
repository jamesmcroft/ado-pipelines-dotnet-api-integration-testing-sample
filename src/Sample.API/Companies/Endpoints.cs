using Microsoft.AspNetCore.Mvc;
using Sample.API.Companies.Models;

namespace Sample.API.Companies;

/// <summary>
/// Defines the endpoints for the Companies API.
/// </summary>
internal static class Endpoints
{
    /// <summary>
    /// Maps the endpoints for the Companies API.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to map the endpoints to.</param>
    /// <returns>The <see cref="WebApplication"/> with the endpoints mapped.</returns>
    internal static WebApplication MapCompaniesEndpoints(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet("Companies API").Build();

        var companiesEndpoints = app.MapGroup("/companies").WithApiVersionSet(versionSet);

        companiesEndpoints.MapGet("/", async (GetCompanies handler) =>
        {
            var companies = await handler.HandleAsync().ConfigureAwait(false);
            return Results.Ok(companies);
        })
        .WithName(nameof(GetCompanies))
        .WithDescription("Retrieve companies")
        .Produces<IEnumerable<CompanyModel>>(StatusCodes.Status200OK);

        companiesEndpoints.MapPost("/", async ([FromBody] CreateCompany.CreateCompanyRequest request, CreateCompany handler) =>
        {
            await handler.HandleAsync(request).ConfigureAwait(false);
            return Results.Ok();
        })
        .WithName(nameof(CreateCompany))
        .WithDescription("Create a new company")
        .Produces(StatusCodes.Status200OK);

        companiesEndpoints.MapDelete("/{id}", async ([FromRoute] int id, DeleteCompany handler) =>
        {
            await handler.HandleAsync(new DeleteCompany.DeleteCompanyRequest { Id = id }).ConfigureAwait(false);
            return Results.Ok();
        })
        .WithName(nameof(DeleteCompany))
        .WithDescription("Delete the company with the specified ID")
        .Produces(StatusCodes.Status200OK);

        return app;
    }

    /// <summary>
    /// Registers the handlers for the Companies API.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to register the handlers to.</param>
    /// <returns>The <see cref="WebApplicationBuilder"/> with the handlers registered.</returns>
    internal static WebApplicationBuilder RegisterCompaniesHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<GetCompanies>();
        builder.Services.AddTransient<CreateCompany>();
        builder.Services.AddTransient<DeleteCompany>();

        return builder;
    }
}
