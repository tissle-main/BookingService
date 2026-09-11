using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Features.Auth.Services;

public interface IAccessTokenGenerator
{
    public abstract ValueTask<string> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken);
}