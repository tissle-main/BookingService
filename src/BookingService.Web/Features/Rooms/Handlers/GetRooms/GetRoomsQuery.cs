using ErrorOr;
using Mediator;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Rooms.Handlers.GetRooms;

/// <summary>Requests rooms, optionally restricted to specific identifiers.</summary>
/// <param name="Ids">The room identifiers to retrieve; an empty array retrieves all rooms.</param>
public sealed record class GetRoomsQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<IEnumerable<RoomDto>>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => GetRoomsEndpoint.AllowedRoles;
    }
    #endregion
}