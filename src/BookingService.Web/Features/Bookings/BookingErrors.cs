using ErrorOr;

namespace BookingService.Web.Features.Bookings;

/// <summary>Creates errors returned by booking operations.</summary>
public static class BookingErrors
{
    /// <summary>Creates an error for booking identifiers that do not exist.</summary>
    /// <param name="missingIds">The missing booking identifiers.</param>
    public static Error NotFound(IEnumerable<Guid> missingIds)
    {
        return Error.NotFound("Booking.NotFound", $"Some of bookings not found in the database: [{string.Join(", ", missingIds)}]");
    }

    /// <summary>Creates an error for an overlapping booking.</summary>
    public static Error Conflict()
    {
        return Error.Conflict("Booking.Conflict", "The selected room is already booked for the requested period.");
    }
}