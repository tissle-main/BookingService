namespace BookingService.Data.Shared.KeyedEntities;

/// <summary>Defines an entity identified by a <see cref="Guid"/>.</summary>
public interface IKeyedEntity
{
    //Value properties
    public abstract Guid Id { get; set; }
}