namespace Sample.API.Shared.API;

using Asp.Versioning;

/// <summary>
/// Defines a set of extension methods for extending the functionality of API versioning.
/// </summary>
public static class VersioningExtensions
{
    /// <summary>
    /// The name of the header that contains the API version.
    /// </summary>
    public const string ApiVersionHeaderName = "x-api-version";

    /// <summary>
    /// Adds API version support to the application.
    /// </summary>
    /// <param name="services">The service collection to add API version support to.</param>
    /// <param name="defaultMajorVersion">The default major version to use. Defaults to 1.</param>
    /// <param name="defaultMinorVersion">The default minor version to use. Defaults to 0.</param>
    /// <returns></returns>
    public static IServiceCollection AddApiVersionSupport(this IServiceCollection services, int defaultMajorVersion = 1,
        int defaultMinorVersion = 0)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(opts =>
            {
                opts.DefaultApiVersion = new ApiVersion(defaultMajorVersion, defaultMinorVersion);
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.ReportApiVersions = true;
                opts.ApiVersionReader = new HeaderApiVersionReader(ApiVersionHeaderName);
            })
            .AddApiExplorer(opts => { opts.GroupNameFormat = "'v'VVV"; });

        return services;
    }
}
