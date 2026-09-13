using BookingService.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Web.Features.Bookings;

internal static class BookingDatabaseLock
{
    internal static async ValueTask AcquireAsync(AppDbContext dbContext, Guid roomId, CancellationToken cancellationToken)
    {
        string resource = $"BookingService:{nameof(dbContext.Rooms)}:{roomId}";
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC sp_getapplock @Resource = {resource}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 5000",
            cancellationToken
        );
    }
}