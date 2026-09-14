using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.SignalR;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

public sealed class UpdateRoomHandler(
    AppDbContext thisDbContext,
    IHubContext<SignalRHub, ISignalRClient> thisSignalR
) : ICommandHandler<UpdateRoomCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(UpdateRoomCommand command, CancellationToken cancellationToken)
    {
        RoomEntity room = command.Room.ToEntity();
        if(await thisDbContext.Rooms.AnyAsync(e => e.Name == room.Name, cancellationToken))
        {
            return RoomErrors.Conflict(room.Name);
        }

        RoomEntity? oldRoom = await thisDbContext.Rooms.FirstOrDefaultAsync(e => e.Id == room.Id, cancellationToken);
        if(oldRoom is null)
        {
            return RoomErrors.NotFound([room.Id]);
        }

        room.MapToEntity(oldRoom);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        await thisSignalR.Clients.All.RoomsUpdated();

        return Unit.Value;
    }
    #endregion
}