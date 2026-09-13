using Microsoft.EntityFrameworkCore;
using BookingService.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Data.Features.Rooms;

public sealed class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(RoomEntityConstants.NameMaxLength);
        builder.HasIndex(e => e.Name).IsUnique();
    }
    #endregion
}