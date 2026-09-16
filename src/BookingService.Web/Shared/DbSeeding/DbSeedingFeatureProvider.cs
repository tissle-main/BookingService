using BookingService.Web.Features;

namespace BookingService.Web.Shared.DbSeeding;

/// <summary>Registers database seeding services and its internal command endpoint.</summary>
public sealed class DbSeedingFeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsEnvironment(ProfileNames.Test))
        {
            app.AddDbSeedingEndpoint();
        }
    }
    #endregion
}