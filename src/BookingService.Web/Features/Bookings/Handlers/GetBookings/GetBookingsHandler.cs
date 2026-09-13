using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Bookings.Dtos;

namespace BookingService.Web.Features.Bookings.Handlers.GetBookings;

public sealed class GetBookingsHandler(AppDbContext thisDbContext) : IQueryHandler<GetBookingsQuery, ErrorOr<IEnumerable<BookingDto>>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<IEnumerable<BookingDto>>> Handle(GetBookingsQuery query, CancellationToken cancellationToken)
    {
        Guid[] ids = query.Ids.Distinct().ToArray();
        if(ids.Length == 0)
        {
            return await thisDbContext.Bookings.AsNoTracking().ProjectToDto().ToArrayAsync(cancellationToken);
        }

        BookingDto[] bookings = await thisDbContext.Bookings.AsNoTracking().Where(e => ids.Contains(e.Id)).ProjectToDto().ToArrayAsync(cancellationToken);
        if(ids.Length > bookings.Length)
        {
            IEnumerable<Guid> existingIds = bookings.Select(b => b.Id);
            return BookingErrors.NotFound(ids.Except(existingIds));
        }
        return bookings;
    }
    #endregion
}