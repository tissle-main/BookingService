using BookingService.Web.Features.Bookings.Handlers.GetBookings;
using BookingService.Web.Features.Bookings.Handlers.CreateBooking;

namespace BookingService.Web.Features.Bookings;

public sealed class BookingFeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsEnvironment(ProfileNames.Test))
        {
            app.AddGetBookingsEndpoint();
            app.AddCreateBookingEndpoint();
        }
    }
    #endregion
}