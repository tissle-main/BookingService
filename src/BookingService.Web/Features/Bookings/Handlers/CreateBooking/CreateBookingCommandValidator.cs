using FluentValidation;
using BookingService.Web.Features.Bookings.Dtos;

namespace BookingService.Web.Features.Bookings.Handlers.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        base.RuleFor(e => e.Booking).SetValidator(new BookingDtoValidator());
    }
}
