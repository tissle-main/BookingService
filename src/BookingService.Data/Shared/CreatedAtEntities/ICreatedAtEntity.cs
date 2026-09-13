namespace BookingService.Data.Shared.CreatedAtEntities;

public interface ICreatedAtEntity
{
    //Value properties
    public abstract DateTime CreatedAt { get; set; }
}