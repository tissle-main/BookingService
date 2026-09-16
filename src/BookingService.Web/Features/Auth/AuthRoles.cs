namespace BookingService.Web.Features.Auth;

/// <summary>Defines roles used by authentication and authorization.</summary>
public static class AuthRoles
{
    /// <summary>Gets the administrator role name.</summary>
    public const string Admin = nameof(Admin);

    /// <summary>Gets the standard user role name.</summary>
    public const string User = nameof(User);

    /// <summary>Gets all roles allowed to use general authenticated operations.</summary>
    public static string[] Any { get; } = [Admin, User];
}