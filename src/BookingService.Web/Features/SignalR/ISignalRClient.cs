namespace BookingService.Web.Features.SignalR;

/// <summary>Describes notifications sent from the SignalR hub to connected clients.</summary>
public interface ISignalRClient
{
    /// <summary>Notifies clients that bookings for a room have changed.</summary>
    /// <param name="RoomId">The affected room identifier.</param>
    public abstract Task BookingsUpdated(Guid RoomId);

    /// <summary>Notifies clients that the room collection has changed.</summary>
    public abstract Task RoomsUpdated();
}