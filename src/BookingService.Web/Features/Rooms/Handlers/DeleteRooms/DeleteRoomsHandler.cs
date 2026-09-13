using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;

namespace BookingService.Web.Features.Rooms.Handlers.DeleteRooms;

public sealed class DeleteRoomsHandler(AppDbContext thisDbContext) : ICommandHandler<DeleteRoomsCommand, ErrorOr<Unit>>
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

        RoomEntity[] rooms = await thisDbContext.Rooms.AsNoTracking().Where(e => ids.Contains(e.Id)).ToArrayAsync(cancellationToken);
        if(ids.Length > rooms.Length)
        {
            IEnumerable<Guid> existingIds = rooms.Select(r => r.Id);
            return RoomErrors.NotFound(ids.Except(existingIds));
        }
        thisDbContext.Rooms.RemoveRange(rooms);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}