using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using Sample.API.Companies.Models;
using Sample.API.Shared.Data;
using Sample.API.Shared.Observability;

namespace Sample.API.Companies;

/// <summary>
/// Defines the handler for retrieving companies.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetCompanies"/> class.
/// </remarks>
[ActivitySource(nameof(GetCompanies))]
public class GetCompanies(ApplicationDbContext dbContext)
{
    private readonly Tracer _tracer = TracerProvider.Default.GetTracer(nameof(GetCompanies));

    /// <summary>
    /// Handles the request to retrieve companies.
    /// </summary>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> to use.</param>
    /// <returns>A set of <see cref="CompanyModel"/> models representing the requested companies.</returns>
    internal async Task<IEnumerable<CompanyModel>> HandleAsync(CancellationToken cancellationToken = default)
    {
        using var span = _tracer.StartActiveSpan(nameof(HandleAsync));

        return await dbContext.Companies
            .AsNoTracking()
            .Select(company => new CompanyModel(company.Id, company.Name))
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}
