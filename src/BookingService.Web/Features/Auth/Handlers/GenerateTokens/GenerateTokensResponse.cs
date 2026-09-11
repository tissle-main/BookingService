using BookingService.Web.Features.Auth.Dtos.RefreshTokens;

namespace BookingService.Web.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensResponse(string AccessToken, RefreshTokenDto RefreshToken);