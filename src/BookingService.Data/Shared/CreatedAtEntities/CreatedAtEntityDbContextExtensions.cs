using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookingService.Data.Shared.CreatedAtEntities;

public static class CreatedAtEntityDbContextExtensions
{
    extension(DbContext thisDbContext)
    {
        public void SetUtcNowForCreatedAtEntities()
        {
            var addedCreatedAtEntities = thisDbContext.ChangeTracker.Entries<ICreatedAtEntity>().Where(entry => entry.State is EntityState.Added);
            foreach(EntityEntry<ICreatedAtEntity> entry in addedCreatedAtEntities)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}