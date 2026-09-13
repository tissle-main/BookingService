using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Features.Rooms.Handlers.GetRooms;

namespace BookingService.Web.Features.Auth.Handlers.GetUsers;

public sealed record class GetUsersQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<IEnumerable<UserDto>>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => GetRoomsEndpoint.AllowedRoles;
    }
    #endregion
}