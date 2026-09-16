using Bogus;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Rooms;

namespace BookingService.Web.Features.Rooms.Dtos;

public static class RoomEntityDbSeeder
{
    extension(Faker<RoomEntity> thisFaker)
    {
        public async ValueTask<RoomEntity[]> SeedDatabaseAsync(AppDbContext db, CancellationToken cancellationToken, int min = 2, int max = 10)
        {
            await db.Rooms.AddRangeAsync(thisFaker.GenerateBetween(min, max), cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            return await db.Rooms.AsNoTracking().ToArrayAsync(cancellationToken);
        }
    }
}