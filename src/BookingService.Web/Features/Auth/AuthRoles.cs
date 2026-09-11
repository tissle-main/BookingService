namespace BookingService.Web.Features.Auth;

public static class AuthRoles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);

    public static string OneOf(params IEnumerable<string> roles)
    {
        return string.Join(", ", roles);
    }
}