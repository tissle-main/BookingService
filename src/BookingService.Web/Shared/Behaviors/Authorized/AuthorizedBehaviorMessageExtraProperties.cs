using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehaviorMessageExtraProperties
{
    public UserEntity User { get; set; } = null!;
}