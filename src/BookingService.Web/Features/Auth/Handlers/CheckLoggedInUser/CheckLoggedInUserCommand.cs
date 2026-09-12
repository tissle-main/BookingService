using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.CheckLoggedInUser;

public sealed record class CheckLoggedInUserCommand(string Email) : IAuthorizedBehaviorMessage, ICommand<ErrorOr<bool>>
{
    #region Interfaces
    public string Role
    {
        get => AuthRoles.OneOf(AuthRoles.User, AuthRoles.Admin);
    }
    #endregion
}
