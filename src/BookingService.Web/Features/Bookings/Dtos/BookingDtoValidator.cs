using FluentValidation;

namespace BookingService.Web.Features.Bookings.Dtos;

public sealed class BookingDtoValidator : AbstractValidator<BookingDto>
{
    public BookingDtoValidator()
    {
        base.RuleFor(dto => dto.BookingDuration).GreaterThan(TimeSpan.Zero);
    }
}