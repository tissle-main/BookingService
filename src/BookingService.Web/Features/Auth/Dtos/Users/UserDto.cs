namespace BookingService.Web.Features.Auth.Dtos.Users;

public sealed class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
}