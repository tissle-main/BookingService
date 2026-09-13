using BookingService.Data.Features.Bookings;
using BookingService.Data.Shared.KeyedEntities;

namespace BookingService.Data.Features.Rooms;

public sealed class RoomEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public string Name { get; set; } = "";

    //Navigation properties
    public List<BookingEntity> Bookings { get; set; } = [];
}