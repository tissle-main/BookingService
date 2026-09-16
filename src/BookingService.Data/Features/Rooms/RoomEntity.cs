using BookingService.Data.Features.Bookings;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Shared.CreatedAtEntities;

namespace BookingService.Data.Features.Rooms;

/// <summary>Represents a bookable room.</summary>
public sealed class RoomEntity : IKeyedEntity, ICreatedAtEntity
{
    //Value properties
    /// <summary>Gets or sets the room identifier.</summary>
    public Guid Id { get; set; } //Interfaces
    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Gets or sets the display name of the room.</summary>
    public string Name { get; set; } = "";

    //Navigation properties
    /// <summary>Gets or sets the bookings associated with the room.</summary>
    public List<BookingEntity> Bookings { get; set; } = [];
}