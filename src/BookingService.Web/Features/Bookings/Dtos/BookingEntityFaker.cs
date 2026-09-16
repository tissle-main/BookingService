using Bogus;
using BookingService.Data.Features.Bookings;

namespace BookingService.Web.Features.Bookings.Dtos;

public static class BookingEntityFaker
{
    private static Faker Faker { get; } = new();

    extension(Faker<BookingEntity> thisFaker)
    {
        public Faker<BookingEntity> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                DateTime bookingStart = g.Date.Soon();
                DateTime bookingEnd = g.Date.Soon(refDate: bookingStart.AddDays(1));
                return new BookingEntity()
                {
                    BookingStart = bookingStart,
                    BookingEnd = bookingEnd
                };
            });
        }
        public Faker<BookingEntity> WithBookingStart(DateTime bookingStart)
        {
            return thisFaker.RuleFor(e => e.BookingStart, bookingStart).RuleFor(e => e.BookingEnd, g =>
            {
                return g.Date.Soon(refDate: bookingStart.AddDays(1));
            });
        }
        public Faker<BookingEntity> WithUserId(Guid userId)
        {
            return thisFaker.RuleFor(e => e.UserId, userId);
        }
        public Faker<BookingEntity> WithRoomId(Guid roomId)
        {
            return thisFaker.RuleFor(e => e.RoomId, roomId);
        }
        public Faker<BookingEntity> WithZeroBookingDuration()
        {
            DateTime dateTime = Faker.Date.Soon();
            return thisFaker.RuleFor(e => e.BookingStart, dateTime).RuleFor(e => e.BookingEnd, dateTime);
        }
    }
}