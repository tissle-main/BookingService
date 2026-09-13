using Microsoft.EntityFrameworkCore;
using BookingService.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingService.Data.Features.Auth.Users.ForeignKey;

namespace BookingService.Data.Features.Bookings;

public sealed class BookingEntityConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.BookingStart).IsRequired();
        builder.Property(e => e.BookingEnd).IsRequired();
        builder.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.ConfigureUserEntityForeignKey(u => u.Bookings, DeleteBehavior.Cascade);
    }
    #endregion
}