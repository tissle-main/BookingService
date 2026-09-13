namespace BookingService.Web.Features.Bookings.Dtos;

public sealed class BookingDto
{
    public Guid Id { get; set; }
    public DateTimeOffset BookingStart { get; set; }
    public TimeSpan BookingDuration { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
}