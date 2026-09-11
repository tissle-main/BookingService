namespace BookingService.Web.Features.Auth.Dtos.RefreshTokens;

public sealed class RefreshTokenDto
{
    public string Value { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}