using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.SignalR;
using BookingService.Data.Features.Bookings;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Bookings.Handlers.CreateBooking;

public sealed class CreateBookingHandler(
    AppDbContext thisDbContext,
    IHubContext<SignalRHub, ISignalRClient> signalr
) : ICommandHandler<CreateBookingCommand, ErrorOr<Guid>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Guid>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        BookingEntity booking = command.Booking.ToEntity();
        
        await BookingDatabaseLock.AcquireAsync(thisDbContext, booking.RoomId, cancellationToken);
        if(await thisDbContext.Bookings.AnyAsync(
            e => e.RoomId == booking.RoomId && e.BookingStart < booking.BookingEnd && e.BookingEnd > booking.BookingStart,
            cancellationToken
        ))
        {
            return BookingErrors.Conflict();
        }

        booking.UserId = user.Id;
        await thisDbContext.Bookings.AddAsync(booking, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        await signalr.Clients.All.BookingsUpdated(booking.RoomId);
        return booking.Id;
    }
    #endregion
}