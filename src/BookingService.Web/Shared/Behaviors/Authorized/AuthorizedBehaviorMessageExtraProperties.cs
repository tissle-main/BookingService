using BookingService.Data.Features.Auth.Roles;
using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehaviorMessageExtraProperties
{
    public UserEntity User { get; set; } = null!;
    public IList<string> Roles { get; set; } = [];
}