namespace BookingService.Web.Features.SignalR;

public interface ISignalRClient
{
    public abstract Task BookingsUpdated(Guid RoomId);
    public abstract Task RoomsUpdated();
}