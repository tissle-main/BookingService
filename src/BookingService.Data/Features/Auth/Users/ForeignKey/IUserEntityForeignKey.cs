namespace BookingService.Data.Features.Auth.Users.ForeignKey;

/// <summary>Defines an entity related to an application user.</summary>
public interface IUserEntityForeignKey
{
    //Value properties
    /// <summary>Gets or sets the related user identifier.</summary>
    public abstract Guid UserId { get; set; }

    //Navigation properties
    /// <summary>Gets or sets the related user, when it is loaded.</summary>
    public abstract UserEntity? User { get; set; }
}