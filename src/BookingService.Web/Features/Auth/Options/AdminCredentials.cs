namespace BookingService.Web.Features.Auth.Options;

public sealed class AdminCredentials
{
    #region Static
    public const string SectionName = "AdminCredentials";
    #endregion

    #region Instance
    public required string Email { get; init; }
    public required string Password { get; init; }
    #endregion
}