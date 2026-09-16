namespace BookingService.Web.Features.Rooms.Dtos;

/// <summary>Data transfer object containing room details and booking identifiers.</summary>
public sealed class RoomDto
{
    /// <summary>Gets or sets the room identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Gets or sets the room creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Gets or sets the display name of the room.</summary>
    public string Name { get; set; } = "";
    /// <summary>Gets or sets the identifiers of bookings associated with the room.</summary>
    public List<Guid> Bookings { get; set; } = [];
}