using Microsoft.AspNetCore.Identity;

namespace BookingService.Data.Features.Auth.Users;

public sealed class UserEntity : IdentityUser<Guid>
{
}