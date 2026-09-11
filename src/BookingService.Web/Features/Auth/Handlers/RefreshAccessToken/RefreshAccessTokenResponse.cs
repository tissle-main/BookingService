using BookingService.Web.Features.Auth.Dtos.Users;

namespace BookingService.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenResponse(UserDto User, string AccessToken);