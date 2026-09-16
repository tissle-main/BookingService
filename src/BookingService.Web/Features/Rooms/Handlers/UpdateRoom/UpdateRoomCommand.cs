using ErrorOr;
using Mediator;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

/// <summary>Requests an update to an existing room.</summary>
/// <param name="Room">The updated room details.</param>
public sealed record class UpdateRoomCommand(RoomDto Room) : IAuthorizedBehaviorMessage, IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => UpdateRoomEndpoint.AllowedRoles;
    }
    #endregion
}