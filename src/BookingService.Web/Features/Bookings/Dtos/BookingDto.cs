namespace BookingService.Web.Features.Bookings.Dtos;

/// <summary>Data transfer object containing booking details.</summary>
public sealed class BookingDto
{
    /// <summary>Gets or sets the booking identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Gets or sets the booking start time.</summary>
    public DateTimeOffset BookingStart { get; set; }
    /// <summary>Gets or sets the booking duration.</summary>
    public TimeSpan BookingDuration { get; set; }
    /// <summary>Gets or sets the booked room identifier.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the identifier of the user who created the booking.</summary>
    public Guid UserId { get; set; }
}