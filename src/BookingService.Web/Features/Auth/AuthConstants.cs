using System.Diagnostics.CodeAnalysis;

namespace BookingService.Web.Features.Auth;

/// <summary>Defines authentication validation constants.</summary>
public static class AuthConstants
{
    /// <summary>Gets the minimum permitted password length.</summary>
    public const int PasswordMinLength = 8;
    /// <summary>Gets the validation message used for invalid passwords.</summary>
    public const string PasswordValidationMessage = "Password must contain at least 8 characters, including uppercase, lowercase, number, and a special character.";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    /// <summary>Gets the regular expression used to validate password complexity.</summary>
    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?`~]{8,}$";
}