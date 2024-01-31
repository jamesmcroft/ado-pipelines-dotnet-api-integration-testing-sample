namespace Sample.API.Shared.Data;

/// <summary>
/// Defines the settings for configuring application data.
/// </summary>
public class DataSettings(string? sqlConnectionString)
{
    /// <summary>
    /// The configuration key for the SQL connection string.
    /// </summary>
    public const string SqlConnectionStringConfigKey = "SqlConnectionString";

    /// <summary>
    /// Gets the SQL connection string.
    /// </summary>
    public string? SqlConnectionString { get; init; } = sqlConnectionString;

    /// <summary>
    /// Creates a new instance of the <see cref="DataSettings"/> class from the specified configuration.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> to use.</param>
    /// <returns>A new instance of the <see cref="DataSettings"/> class.</returns>
    public static DataSettings FromConfiguration(IConfiguration configuration)
    {
        return new DataSettings(
            configuration[SqlConnectionStringConfigKey]);
    }
}
