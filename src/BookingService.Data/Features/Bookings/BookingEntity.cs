using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Features.Auth.Users.ForeignKey;

namespace BookingService.Data.Features.Bookings;

/// <summary>Represents a reservation of a room by a user.</summary>
public sealed class BookingEntity : IKeyedEntity, IUserEntityForeignKey
{
    //Value properties
    /// <summary>Gets or sets the booking identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Gets or sets the inclusive start time of the booking.</summary>
    public DateTimeOffset BookingStart { get; set; }
    /// <summary>Gets or sets the exclusive end time of the booking.</summary>
    public DateTimeOffset BookingEnd { get; set; }
    /// <summary>Gets or sets the booked room identifier.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the user who created the booking.</summary>
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    /// <summary>Gets or sets the associated room, when it is loaded.</summary>
    public RoomEntity? Room { get; set; }
    /// <summary>Gets or sets the associated user, when it is loaded.</summary>
    public UserEntity? User { get; set; } //Interfaces
}