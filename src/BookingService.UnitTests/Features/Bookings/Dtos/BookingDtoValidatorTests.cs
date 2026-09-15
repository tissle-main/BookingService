using Bogus;
using FluentValidation.TestHelper;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Bookings.Dtos;

namespace BookingService.UnitTests.Features.Bookings.Dtos;

public sealed class BookingDtoValidatorTests
{
    private BookingDtoValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        BookingDto dto = new Faker<BookingEntity>().ValidInstance().Generate().ToDto();

        //Act
        TestValidationResult<BookingDto> result = await Validator.TestValidateAsync(dto, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenBookingDurationIsZero(CancellationToken cancellationToken)
    {
        //Arrange
        BookingDto dto = new Faker<BookingEntity>().ValidInstance().WithZeroBookingDuration().Generate().ToDto();

        //Act
        TestValidationResult<BookingDto> result = await Validator.TestValidateAsync(dto, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.BookingDuration);
    }
}