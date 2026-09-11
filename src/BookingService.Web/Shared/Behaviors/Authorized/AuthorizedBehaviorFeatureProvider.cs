using BookingService.Web.Features;

namespace BookingService.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehaviorFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
    }
    #endregion
}