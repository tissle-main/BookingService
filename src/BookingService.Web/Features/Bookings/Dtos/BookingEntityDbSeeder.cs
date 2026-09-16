using Bogus;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Bookings;

namespace BookingService.Web.Features.Bookings.Dtos;

public static class BookingEntityDbSeeder
{
    private static Faker Faker { get; } = new();

    extension(Faker<BookingEntity> thisFaker)
    {
        public async ValueTask<BookingEntity[]> SeedDatabaseForUserAndRoomAsync(AppDbContext db, CancellationToken cancellationToken, int min = 2, int max = 5)
        {
            for(int count = Faker.Random.Number(min, max) - 1; count >= 0; count--)
            {
                DateTime bookingStart = DateTime.UtcNow.AddDays(count * 2);
                BookingEntity entity = thisFaker.Clone().WithBookingStart(bookingStart);
                await db.Bookings.AddAsync(entity, cancellationToken);
            }
            await db.SaveChangesAsync(cancellationToken);
            return await db.Bookings.AsNoTracking().ToArrayAsync(cancellationToken);
        }
        public async ValueTask<BookingEntity[]> SeedDatabaseForUser(AppDbContext db, CancellationToken cancellationToken, int min = 2, int max = 5)
        {
            Guid[] roomIds = await db.Rooms.AsNoTracking().Select(e => e.Id).ToArrayAsync(cancellationToken);
            foreach(Guid roomId in roomIds)
            {
                await thisFaker.Clone().WithRoomId(roomId).SeedDatabaseForUserAndRoomAsync(db, cancellationToken, min, max);
            }
            return await db.Bookings.AsNoTracking().ToArrayAsync(cancellationToken);
        }
    }
}