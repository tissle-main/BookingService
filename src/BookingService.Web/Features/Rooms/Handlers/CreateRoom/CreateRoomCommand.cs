using ErrorOr;
using Mediator;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

public sealed record class CreateRoomCommand(RoomDto Room) : IAuthorizedBehaviorMessage, IDbTransactionBehaviorMessage, ICommand<ErrorOr<Guid>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => CreateRoomEndpoint.AllowedRoles;
    }
    #endregion
}