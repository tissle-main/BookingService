using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Features.Auth.Users.ForeignKey;

namespace BookingService.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntity : IKeyedEntity, IUserEntityForeignKey
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public string Value { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; } //Interfaces

    //Navigation properties
    public UserEntity? User { get; set; } //Interfaces
}