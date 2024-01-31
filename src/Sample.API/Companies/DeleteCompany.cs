using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using Sample.API.Shared.Data;
using Sample.API.Shared.Observability;

namespace Sample.API.Companies;

/// <summary>
/// Defines the handler for deleting a company by ID.
/// </summary>
[ActivitySource(nameof(DeleteCompany))]
public class DeleteCompany(ApplicationDbContext dbContext)
{
    private readonly Tracer _tracer = TracerProvider.Default.GetTracer(nameof(DeleteCompany));

    /// <summary>
    /// Handles the request to delete a company by ID.
    /// </summary>
    /// <param name="request">The details of the company to delete.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> to use.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal async Task HandleAsync(DeleteCompanyRequest request, CancellationToken cancellationToken = default)
    {
        using var span = _tracer.StartActiveSpan(nameof(HandleAsync));

        var company = await dbContext.Companies
            .FirstOrDefaultAsync(company => company.Id == request.Id, cancellationToken).ConfigureAwait(false);

        if (company is not null)
        {
            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Defines the request to delete a company.
    /// </summary>
    public record DeleteCompanyRequest
    {
        /// <summary>
        /// Gets or sets the ID of the company.
        /// </summary>
        public required int Id { get; set; }
    }
}
