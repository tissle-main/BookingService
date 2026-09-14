using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.SignalR;

namespace BookingService.Web.Features.Rooms.Handlers.DeleteRooms;

public sealed class DeleteRoomsHandler(
    AppDbContext thisDbContext,
    IHubContext<SignalRHub, ISignalRClient> thisSignalR
) : ICommandHandler<DeleteRoomsCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteRoomsCommand command, CancellationToken cancellationToken)
    {
        Guid[] ids = command.Ids.Distinct().ToArray();
        if(ids.Length == 0)
        {
            thisDbContext.Rooms.RemoveRange(thisDbContext.Rooms);
            await thisDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        RoomEntity[] rooms = await thisDbContext.Rooms.Where(e => ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
        if(ids.Length > rooms.Length)
        {
            IEnumerable<Guid> existingIds = rooms.Select(r => r.Id);
            return RoomErrors.NotFound(ids.Except(existingIds));
        }
        thisDbContext.Rooms.RemoveRange(rooms);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        await thisSignalR.Clients.All.RoomsUpdated();
        return Unit.Value;
    }
    #endregion
}