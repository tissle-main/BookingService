using System.Diagnostics.CodeAnalysis;

namespace BookingService.Web.Features.Auth;

public static class AuthConstants
{
    public const int PasswordMinLength = 8;
    public const string PasswordValidationMessage = "Password must contain at least 8 characters, including uppercase, lowercase, number, and a special character.";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?`~]{8,}$";
}