using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Features.Auth.Users.ForeignKey;

namespace BookingService.Data.Features.Bookings;

public sealed class BookingEntity : IKeyedEntity, IUserEntityForeignKey
{
    //Value properties
    public Guid Id { get; set; }
    public DateTimeOffset BookingStart { get; set; }
    public DateTimeOffset BookingEnd { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    public RoomEntity? Room { get; set; }
    public UserEntity? User { get; set; } //Interfaces
}