using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sample.API.Companies.Entities;

namespace Sample.API.Shared.Data;

/// <summary>
/// Defines the application database context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationDbContext"/> class with the specified options.
/// </remarks>
/// <param name="options">The options to configure for the database context.</param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the companies in the database.
    /// </summary>
    public DbSet<Company> Companies { get; set; }

    /// <summary>
    /// Gets the assembly containing the database migrations.
    /// </summary>
    protected Assembly MigrationsAssembly => typeof(ApplicationDbContext).Assembly;

    /// <summary>
    /// Searches for one character expression inside a second character expression to retrieve the starting position of the first character expression.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <param name="columnValue">The value to search in.</param>
    /// <returns>The starting position of the first character expression in the second character expression.</returns>
    [DbFunction("CHARINDEX", IsBuiltIn = true)]
    public static long SqlCharIndex(string searchTerm, string columnValue) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(MigrationsAssembly);
    }
}
