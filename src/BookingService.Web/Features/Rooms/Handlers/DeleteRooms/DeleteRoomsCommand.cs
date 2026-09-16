using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Rooms.Handlers.DeleteRooms;

/// <summary>Requests deletion of rooms by identifier.</summary>
/// <param name="Ids">The room identifiers to delete.</param>
public sealed record class DeleteRoomsCommand(Guid[] Ids) : IAuthorizedBehaviorMessage, IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => DeleteRoomsEndpoint.AllowedRoles;
    }
    #endregion
}