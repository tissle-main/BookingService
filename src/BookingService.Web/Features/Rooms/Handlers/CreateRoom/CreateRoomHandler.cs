using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.SignalR;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

public sealed class CreateRoomHandler(
    AppDbContext thisDbContext,
    IHubContext<SignalRHub, ISignalRClient> thisSignalR
) : ICommandHandler<CreateRoomCommand, ErrorOr<Guid>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Guid>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        RoomEntity room = command.Room.ToEntity();
        if(await thisDbContext.Rooms.AnyAsync(e => e.Name == room.Name, cancellationToken))
        {
            return RoomErrors.Conflict(room.Name);
        }

        await thisDbContext.Rooms.AddAsync(room, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        await thisSignalR.Clients.All.RoomsUpdated();
        return room.Id;
    }
    #endregion
}