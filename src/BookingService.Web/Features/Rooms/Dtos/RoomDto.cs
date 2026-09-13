namespace BookingService.Web.Features.Rooms.Dtos;

public sealed class RoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<Guid> Bookings { get; set; } = [];
}