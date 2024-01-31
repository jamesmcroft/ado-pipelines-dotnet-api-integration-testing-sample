using OpenTelemetry.Trace;
using Sample.API.Companies.Entities;
using Sample.API.Shared.Data;
using Sample.API.Shared.Observability;

namespace Sample.API.Companies;

/// <summary>
/// Defines the handler for adding a company.
/// </summary>
[ActivitySource(nameof(CreateCompany))]
public class CreateCompany(ApplicationDbContext dbContext)
{
    private readonly Tracer _tracer = TracerProvider.Default.GetTracer(nameof(CreateCompany));

    /// <summary>
    /// Handles the request to create a company.
    /// </summary>
    /// <param name="request">The details of the index to create or update.</param>
    /// <param name="cancellationToken">The optional <see cref="CancellationToken"/> to use.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal async Task HandleAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default)
    {
        using var span = _tracer.StartActiveSpan(nameof(HandleAsync));

        var company = new Company { Name = request.Name };

        dbContext.Companies.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Defines the request to create a company.
    /// </summary>
    public record CreateCompanyRequest
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public required string Name { get; set; }
    }
}
