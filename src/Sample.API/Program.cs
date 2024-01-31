using Sample.API.Companies;
using Sample.API.Shared.API;
using Sample.API.Shared.Data;
using Sample.API.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

var observabilitySettings = ObservabilitySettings.FromConfiguration(builder.Configuration);
builder.Services.AddScoped(_ => observabilitySettings);

var dataSettings = DataSettings.FromConfiguration(builder.Configuration);
builder.Services.AddScoped(_ => dataSettings);

builder.ConfigureObservability(observabilitySettings);
builder.ConfigureApplicationDatabase(dataSettings);

builder.Services.AddApiVersionSupport();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

builder.RegisterCompaniesHandlers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapCompaniesEndpoints();
app.MigrateApplicationDatabase();

app.Run();

/// <summary>
/// Defines the entry point for the application.
/// </summary>
/// <remarks>
/// This is required for the integration tests to run.
/// </remarks>
public partial class Program;
