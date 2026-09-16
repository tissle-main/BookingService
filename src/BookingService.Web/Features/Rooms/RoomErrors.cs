using ErrorOr;

namespace BookingService.Web.Features.Rooms;

/// <summary>Creates errors returned by room operations.</summary>
public static class RoomErrors
{
    /// <summary>Creates an error for room identifiers that do not exist.</summary>
    /// <param name="missingIds">The missing room identifiers.</param>
    public static Error NotFound(IEnumerable<Guid> missingIds)
    {
        return Error.NotFound("Room.NotFound", $"Some of rooms not found in the database: [{string.Join(", ", missingIds)}]");
    }

    /// <summary>Creates an error for a duplicate room name.</summary>
    /// <param name="name">The conflicting room name.</param>
    public static Error Conflict(string name)
    {
        return Error.Conflict("Room.Conflict", $"Room with name '{name}' already exists.");
    }
}