using Bogus;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Bookings.Dtos;

namespace BookingService.Web.Features.Bookings.Handlers.CreateBooking;

public static class CreateBookingCommandFaker
{
    extension(Faker<CreateBookingCommand> thisFaker)
    {
        public Faker<CreateBookingCommand> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                BookingDto dto = new Faker<BookingEntity>().ValidInstance().Generate().ToDto();
                return new CreateBookingCommand(dto);
            });
        }
        public Faker<CreateBookingCommand> WithBooking(BookingDto booking)
        {
            return thisFaker.RuleFor(c => c.Booking, booking);
        }
    }
}