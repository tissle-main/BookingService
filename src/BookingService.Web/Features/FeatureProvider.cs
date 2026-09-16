namespace BookingService.Web.Features;

/// <summary>Provides a composition boundary for a web application feature.</summary>
public abstract class FeatureProvider
{
    /// <summary>Registers services required by the feature.</summary>
    /// <param name="builder">The web application builder.</param>
    public virtual void AddServices(WebApplicationBuilder builder)
    {

    }

    /// <summary>Registers middleware and endpoints required by the feature.</summary>
    /// <param name="app">The built web application.</param>
    public virtual void UseMiddleware(WebApplication app)
    {

    }
}