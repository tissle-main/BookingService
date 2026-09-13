using BookingService.Data.Features.Bookings;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Shared.CreatedAtEntities;

namespace BookingService.Data.Features.Rooms;

public sealed class RoomEntity : IKeyedEntity, ICreatedAtEntity
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = "";

    //Navigation properties
    public List<BookingEntity> Bookings { get; set; } = [];
}