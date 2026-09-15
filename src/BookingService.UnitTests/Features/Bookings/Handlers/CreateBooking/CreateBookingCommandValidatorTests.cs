using Bogus;
using FluentValidation.TestHelper;
using BookingService.Data.Features.Bookings;
using BookingService.Web.Features.Bookings.Dtos;
using BookingService.UnitTests.Features.Bookings.Dtos;
using BookingService.Web.Features.Bookings.Handlers.CreateBooking;

namespace BookingService.UnitTests.Features.Bookings.Handlers.CreateBooking;

public sealed class CreateBookingCommandValidatorTests
{
    private CreateBookingCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance();

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [DependsOn<BookingDtoValidatorTests>(nameof(BookingDtoValidatorTests.Validator_ShouldNotPass_WhenBookingDurationIsZero))]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        BookingDto booking = new Faker<BookingEntity>().ValidInstance().WithZeroBookingDuration().Generate().ToDto();
        CreateBookingCommand command = new Faker<CreateBookingCommand>().ValidInstance().WithBooking(booking);

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}