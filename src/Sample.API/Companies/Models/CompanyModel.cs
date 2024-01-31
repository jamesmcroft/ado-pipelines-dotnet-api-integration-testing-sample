namespace Sample.API.Companies.Models;

/// <summary>
/// Defines the model for a company.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CompanyModel"/> class with the required properties.
/// </remarks>
/// <param name="id">The unique identifier for the company.</param>
/// <param name="name">The name of the company.</param>
public class CompanyModel(int id, string name)
{
    /// <summary>
    /// Gets or sets the unique identifier for the company.
    /// </summary>
    public int Id { get; set; } = id;

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    public string Name { get; set; } = name;
}
