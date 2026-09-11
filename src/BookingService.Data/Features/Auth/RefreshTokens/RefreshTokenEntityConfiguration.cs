using Microsoft.EntityFrameworkCore;
using BookingService.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingService.Data.Features.Auth.Users.ForeignKey;

namespace BookingService.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.Value).IsRequired().HasMaxLength(RefreshTokenEntityConstants.RefreshTokenMaxLength);
        builder.HasIndex(e => e.Value).IsUnique();
        builder.Property(e => e.ExpiresAt).IsRequired();
        builder.ConfigureUserEntityForeignKey(e => e.RefreshTokens);
    }
    #endregion
}