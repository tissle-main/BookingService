using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookingService.Data.Shared.KeyedEntities;

public static class KeyedEntityDbContextExtensions
{
    extension(DbContext thisDbContext)
    {
        public void GenerateIdForKeyedEntities()
        {
            var addedKeyedEntities = thisDbContext.ChangeTracker.Entries<IKeyedEntity>().Where(entry => entry.State is EntityState.Added);
            foreach(EntityEntry<IKeyedEntity> entry in addedKeyedEntities)
            {
                entry.Entity.Id = Guid.CreateVersion7();
            }
        }
    }
}