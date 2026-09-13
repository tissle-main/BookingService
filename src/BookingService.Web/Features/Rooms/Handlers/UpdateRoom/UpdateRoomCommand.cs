using ErrorOr;
using Mediator;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

public sealed record class UpdateRoomCommand(RoomDto Room) : IAuthorizedBehaviorMessage, IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => UpdateRoomEndpoint.AllowedRoles;
    }
    #endregion
}