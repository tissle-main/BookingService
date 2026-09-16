using ErrorOr;
using Mediator;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Bookings.Handlers.CreateBooking;

/// <summary>Requests creation of a room booking.</summary>
/// <param name="Booking">The booking details to persist.</param>
public sealed record class CreateBookingCommand(BookingDto Booking) : IAuthorizedBehaviorMessage, IDbTransactionBehaviorMessage, ICommand<ErrorOr<Guid>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => CreateBookingEndpoint.AllowedRoles;
    }
    #endregion
}
