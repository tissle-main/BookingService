using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Bookings;

namespace BookingService.Data.Features.Auth.Users;

public sealed class UserEntity : IdentityUser<Guid>
{
    //Navigation properties
    public List<BookingEntity> Bookings { get; set; } = [];
}