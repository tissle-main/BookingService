using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Data.Shared.CreatedAtEntities;

public static class CreatedAtEntityConfiguration
{
    extension<TEntity>(EntityTypeBuilder<TEntity> thisBuilder) where TEntity : class, ICreatedAtEntity
    {
        public void ConfigureCreatedAtEntity()
        {
            thisBuilder.Property(e => e.CreatedAt).IsRequired();
        }
    }
}