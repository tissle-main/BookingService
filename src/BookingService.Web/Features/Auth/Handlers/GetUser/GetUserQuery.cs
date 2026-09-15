using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.GetUser;

public sealed record class GetUserQuery : IAuthorizedBehaviorMessage, IQuery<ErrorOr<UserDto>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => GetUserEndpoint.AllowedRoles;
    }
    #endregion
}