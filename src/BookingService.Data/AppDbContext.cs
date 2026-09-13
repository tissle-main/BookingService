using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;
using BookingService.Data.Features.Bookings;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Features.Auth.Roles;
using BookingService.Data.Shared.KeyedEntities;
using BookingService.Data.Shared.CreatedAtEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookingService.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
{
    #region Instance
    public DbSet<RoomEntity> Rooms { get; set; } = null!; //Init by EF Core
    public DbSet<BookingEntity> Bookings { get; set; } = null!; //Init by EF Core
    #endregion

    #region Base
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.GenerateIdForKeyedEntities();
        this.SetUtcNowForCreatedAtEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.GenerateIdForKeyedEntities();
        this.SetUtcNowForCreatedAtEntities();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    #endregion
}