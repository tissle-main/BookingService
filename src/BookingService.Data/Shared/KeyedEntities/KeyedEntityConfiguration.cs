using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Data.Shared.KeyedEntities;

public static class KeyedEntityConfiguration
{
    extension<TEntity>(EntityTypeBuilder<TEntity> thisBuilder) where TEntity : class, IKeyedEntity
    {
        public void ConfigureKeyedEntity()
        {
            thisBuilder.HasKey(e => e.Id);
            thisBuilder.Property(e => e.Id).IsRequired();
        }
    }
}