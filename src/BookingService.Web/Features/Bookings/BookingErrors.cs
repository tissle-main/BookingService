using ErrorOr;

namespace BookingService.Web.Features.Bookings;

public static class BookingErrors
{
    public static Error NotFound(IEnumerable<Guid> missingIds)
    {
        return Error.NotFound("Booking.NotFound", $"Some of bookings not found in the database: [{string.Join(", ", missingIds)}]");
    }
    public static Error Conflict()
    {
        return Error.Conflict("Booking.Conflict", "The selected room is already booked for the requested period.");
    }
}