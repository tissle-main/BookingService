using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#pragma warning disable IDE0130 
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    extension<TBuilder>(TBuilder thisBuilder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddServiceDefaults()
        {
            thisBuilder.ConfigureOpenTelemetry();
            thisBuilder.AddDefaultHealthChecks();
            thisBuilder.Services.AddServiceDiscovery();
            thisBuilder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });
            return thisBuilder;
        }
        public TBuilder ConfigureOpenTelemetry()
        {
            thisBuilder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });
            thisBuilder.Services.AddOpenTelemetry().WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation();
            }).WithTracing(tracing =>
            {
                tracing.AddSource(thisBuilder.Environment.ApplicationName).AddAspNetCoreInstrumentation(tracing =>
                {
                    tracing.Filter = static bool (HttpContext context) =>
                    {
                        bool isNotHealthEndpointPath = !context.Request.Path.StartsWithSegments(HealthEndpointPath);
                        bool isNotAlivenessEndpointPath = !context.Request.Path.StartsWithSegments(AlivenessEndpointPath);
                        return isNotHealthEndpointPath && isNotAlivenessEndpointPath;
                    };
                }).AddHttpClientInstrumentation();
            });
            thisBuilder.AddOpenTelemetryExporters();
            return thisBuilder;
        }
        private TBuilder AddOpenTelemetryExporters()
        {
            bool useOtlpExporter = !string.IsNullOrWhiteSpace(thisBuilder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            if(useOtlpExporter)
            {
                thisBuilder.Services.AddOpenTelemetry().UseOtlpExporter();
            }
            return thisBuilder;
        }
        public TBuilder AddDefaultHealthChecks()
        {
            thisBuilder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
            return thisBuilder;
        }
    }
    extension(WebApplication thisApp)
    {
        public WebApplication MapDefaultEndpoints()
        {
            if(thisApp.Environment.IsDevelopment())
            {
                thisApp.MapHealthChecks(HealthEndpointPath);
                thisApp.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
                {
                    Predicate = static bool(r) =>
                    {
                        return r.Tags.Contains("live");
                    }
                });
            }
            return thisApp;
        }
    }  
}