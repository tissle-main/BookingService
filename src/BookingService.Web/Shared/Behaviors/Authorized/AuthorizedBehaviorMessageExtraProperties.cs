using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Shared.Behaviors.Authorized;

/// <summary>Stores authenticated user data attached to an authorized message.</summary>
public sealed class AuthorizedBehaviorMessageExtraProperties
{
    public UserEntity User { get; set; } = null!;
    public IList<string> Roles { get; set; } = [];
}