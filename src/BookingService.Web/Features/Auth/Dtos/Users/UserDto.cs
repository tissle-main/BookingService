namespace BookingService.Web.Features.Auth.Dtos.Users;

/// <summary>Data transfer object containing public user details.</summary>
public sealed class UserDto
{
    /// <summary>Gets or sets the user identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = "";
}