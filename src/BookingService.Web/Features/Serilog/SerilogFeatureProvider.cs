using Serilog;

namespace BookingService.Web.Features.Serilog;

/// <summary>Configures Serilog integration for the web application.</summary>
public sealed class SerilogFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, provider, cfg) =>
        {
            cfg.WriteTo.Console();
        });
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseSerilogRequestLogging();
    }
    #endregion
}