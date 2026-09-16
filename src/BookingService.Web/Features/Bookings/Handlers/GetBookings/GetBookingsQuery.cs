using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Bookings.Handlers.GetBookings;

/// <summary>Requests bookings, optionally restricted to specific identifiers.</summary>
/// <param name="Ids">The booking identifiers to retrieve; an empty array retrieves all bookings.</param>
public sealed record class GetBookingsQuery(Guid[] Ids) : IAuthorizedBehaviorMessage, IQuery<ErrorOr<IEnumerable<BookingDto>>>
{
    #region Interfaces
    public string[] AllowedRoles
    {
        get => GetBookingsEndpoint.AllowedRoles;
    }
    #endregion
}