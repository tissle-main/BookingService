using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.GetRooms;

public sealed class GetRoomsHandler(AppDbContext thisDbContext) : IQueryHandler<GetRoomsQuery, ErrorOr<IEnumerable<RoomDto>>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<IEnumerable<RoomDto>>> Handle(GetRoomsQuery query, CancellationToken cancellationToken)
    {
        Guid[] ids = query.Ids.Distinct().ToArray();
        if(ids.Length == 0)
        {
            return await thisDbContext.Rooms.AsNoTracking().ProjectToDto().ToArrayAsync(cancellationToken);
        }

        RoomDto[] rooms = await thisDbContext.Rooms.AsNoTracking().Where(e => ids.Contains(e.Id)).ProjectToDto().ToArrayAsync(cancellationToken);
        if(ids.Length > rooms.Length)
        {
            IEnumerable<Guid> existingIds = rooms.Select(r => r.Id);
            return RoomErrors.NotFound(ids.Except(existingIds));
        }
        return rooms;
    }
    #endregion
}