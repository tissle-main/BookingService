using Microsoft.AspNetCore.Identity;

namespace BookingService.Data.Features.Auth.Roles;

/// <summary>Represents an application role backed by ASP.NET Core Identity.</summary>
public sealed class RoleEntity : IdentityRole<Guid>;