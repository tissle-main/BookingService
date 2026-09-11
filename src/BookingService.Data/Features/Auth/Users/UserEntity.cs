using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.RefreshTokens;

namespace BookingService.Data.Features.Auth.Users;

public sealed class UserEntity : IdentityUser<Guid>
{
    //Navigation properties
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
}