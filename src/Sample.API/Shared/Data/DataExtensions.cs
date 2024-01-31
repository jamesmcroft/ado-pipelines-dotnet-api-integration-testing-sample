using Microsoft.EntityFrameworkCore;

namespace Sample.API.Shared.Data;

/// <summary>
/// Defines a set of extension methods for extending the functionality of application data.
/// </summary>
public static class DataExtensions
{
    /// <summary>
    /// Configures the application database.
    /// </summary>
    /// <param name="appBuilder">The <see cref="WebApplicationBuilder"/> to use.</param>
    /// <param name="dataSettings">The configuration settings to use.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder ConfigureApplicationDatabase(
        this WebApplicationBuilder appBuilder,
        DataSettings dataSettings)
    {
        appBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(dataSettings.SqlConnectionString,
                builder => { builder.EnableRetryOnFailure(); }));

        return appBuilder;
    }

    /// <summary>
    /// Migrates the application database.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to migrate the database for.</param>
    /// <returns>The <see cref="WebApplication"/>.</returns>
    public static WebApplication MigrateApplicationDatabase(this WebApplication app)
    {
        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        dbContext?.Database.Migrate();

        return app;
    }
}
