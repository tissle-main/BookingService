using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Bookings.Handlers.GetBookings;

public sealed record class GetBookingsQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<IEnumerable<BookingDto>>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => GetBookingsEndpoint.AllowedRoles;
    }
    #endregion
}