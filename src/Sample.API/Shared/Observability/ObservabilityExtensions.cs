namespace Sample.API.Shared.Observability;

using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

/// <summary>
/// Defines a set of extension methods for extending the functionality of application observability.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Configures the application logging and telemetry.
    /// </summary>
    /// <param name="appBuilder">The host application builder.</param>
    /// <param name="observabilitySettings">The settings for configuring application logging and telemetry.</param>
    /// <returns>The host application builder.</returns>
    public static WebApplicationBuilder ConfigureObservability(this WebApplicationBuilder appBuilder,
        ObservabilitySettings observabilitySettings)
    {
        ConfigureLogging(appBuilder, observabilitySettings);
        ConfigureOpenTelemetry(appBuilder, observabilitySettings);

        return appBuilder;
    }

    private static WebApplicationBuilder ConfigureLogging(WebApplicationBuilder appBuilder,
        ObservabilitySettings observabilitySettings)
    {
        appBuilder.Services.AddLogging(logBuilder =>
        {
            logBuilder.AddOpenTelemetry(otOpts =>
            {
                if (!string.IsNullOrEmpty(observabilitySettings.ApplicationInsightsConnectionString))
                {
                    otOpts.AddAzureMonitorLogExporter(amOpts =>
                        amOpts.ConnectionString = observabilitySettings.ApplicationInsightsConnectionString);
                }

                otOpts.AddConsoleExporter();
                otOpts.AddOtlpExporter();
                otOpts.IncludeFormattedMessage = true;
            });

            logBuilder.AddConsole();
            logBuilder.SetMinimumLevel(appBuilder.Environment.IsDevelopment()
                ? LogLevel.Information
                : LogLevel.Warning);
        });

        return appBuilder;
    }

    private static WebApplicationBuilder ConfigureOpenTelemetry(WebApplicationBuilder appBuilder,
        ObservabilitySettings observabilitySettings)
    {
        void EnrichActivity(Activity activity)
        {
            activity.SetTag("service.name", appBuilder.Environment.ApplicationName);
            activity.SetTag("service.environment", appBuilder.Environment.EnvironmentName);
        }

        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        if (!string.IsNullOrEmpty(observabilitySettings.ApplicationInsightsConnectionString))
        {
            appBuilder.Services.AddApplicationInsightsTelemetry(opts =>
            {
                opts.ConnectionString = observabilitySettings.ApplicationInsightsConnectionString;
                opts.EnableAdaptiveSampling = false;
                opts.EnableQuickPulseMetricStream = false;
            });
        }

        appBuilder.Services.AddOpenTelemetry().WithTracing(tracerBuilder =>
            {
                tracerBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(appBuilder.Environment.ApplicationName).AddTelemetrySdk()
                    .AddEnvironmentVariableDetector());

                AddActivitySources(tracerBuilder);
                // Add additional external sources here

                tracerBuilder.SetSampler(new AlwaysOnSampler());

                tracerBuilder.AddAspNetCoreInstrumentation(opts =>
                {
                    opts.EnrichWithHttpRequest = (activity, request) =>
                    {
                        EnrichActivity(activity);
                        activity.SetTag("http.method", request.Method);
                        activity.SetTag("http.url", request.Path);
                    };
                });

                tracerBuilder.AddHttpClientInstrumentation(opts =>
                {
                    opts.EnrichWithHttpWebRequest = (activity, request) =>
                    {
                        EnrichActivity(activity);
                        activity.SetTag("http.method", request.Method);
                        activity.SetTag("http.url", request.RequestUri.ToString());
                    };
                });

                // Add additional instrumentation here (e.g. SQL, Entity Framework, etc.)

                tracerBuilder.AddConsoleExporter();
                tracerBuilder.AddOtlpExporter();

                if (!string.IsNullOrEmpty(observabilitySettings.ApplicationInsightsConnectionString))
                {
                    tracerBuilder.AddAzureMonitorTraceExporter(opts =>
                    {
                        opts.Diagnostics.IsLoggingEnabled = true;
                        opts.Diagnostics.IsTelemetryEnabled = true;
                        opts.Diagnostics.IsDistributedTracingEnabled = true;
                        opts.Diagnostics.IsLoggingContentEnabled = true;
                        opts.ConnectionString = observabilitySettings.ApplicationInsightsConnectionString;
                    });
                }

                if (appBuilder.Environment.IsDevelopment())
                {
                    tracerBuilder.AddZipkinExporter(opts =>
                    {
                        opts.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                    });
                }
            })
            .WithMetrics(metricsBuilder =>
            {
                AddMeters(metricsBuilder);
                // Add additional external meters here

                metricsBuilder.AddConsoleExporter();
                metricsBuilder.AddOtlpExporter();

                if (!string.IsNullOrEmpty(observabilitySettings.ApplicationInsightsConnectionString))
                {
                    metricsBuilder.AddAzureMonitorMetricExporter(opts =>
                    {
                        opts.ConnectionString = observabilitySettings.ApplicationInsightsConnectionString;
                    });
                }
            });

        appBuilder.Services.AddSingleton(new ActivitySource(appBuilder.Environment.ApplicationName));
        appBuilder.Services.AddSingleton(TracerProvider.Default.GetTracer(appBuilder.Environment.ApplicationName));

        return appBuilder;
    }

    private static TracerProviderBuilder AddActivitySources(this TracerProviderBuilder builder)
    {
        foreach (var activitySource in ActivitySourceAttribute.GetActivitySourceNames())
        {
            builder.AddSource(activitySource);
        }

        return builder;
    }

    private static MeterProviderBuilder AddMeters(this MeterProviderBuilder builder)
    {
        foreach (var meter in MeterAttribute.GetMeterNames())
        {
            builder.AddMeter(meter);
        }

        return builder;
    }
}
