using ErrorOr;

namespace BookingService.Web.Features.Rooms;

public static class RoomErrors
{
    public static Error NotFound(IEnumerable<Guid> missingIds)
    {
        return Error.NotFound("Room.NotFound", $"Some of rooms not found in the database: [{string.Join(", ", missingIds)}]");
    }
    public static Error Conflict(string name)
    {
        return Error.Conflict("Room.Conflict", $"Room with name '{name}' already exists.");
    }
}