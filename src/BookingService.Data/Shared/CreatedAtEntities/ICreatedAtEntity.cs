namespace BookingService.Data.Shared.CreatedAtEntities;

/// <summary>Defines an entity that records when it was created.</summary>
public interface ICreatedAtEntity
{
    //Value properties
    public abstract DateTime CreatedAt { get; set; }
}