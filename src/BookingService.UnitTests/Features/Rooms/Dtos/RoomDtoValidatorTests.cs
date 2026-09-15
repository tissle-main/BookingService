using Bogus;
using FluentValidation.TestHelper;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.UnitTests.Features.Rooms.Dtos;

public sealed class RoomDtoValidatorTests
{
    private RoomDtoValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        RoomDto dto = new Faker<RoomEntity>().ValidInstance().Generate().ToDto();

        //Act
        TestValidationResult<RoomDto> result = await Validator.TestValidateAsync(dto, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenNameIsEmpty(CancellationToken cancellationToken)
    {
        //Arrange
        RoomDto dto = new Faker<RoomEntity>().ValidInstance().WithEmptyName().Generate().ToDto();

        //Act
        TestValidationResult<RoomDto> result = await Validator.TestValidateAsync(dto, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Name);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenNameIsTooLarge(CancellationToken cancellationToken)
    {
        //Arrange
        RoomDto dto = new Faker<RoomEntity>().ValidInstance().WithTooLargeName().Generate().ToDto();

        //Act
        TestValidationResult<RoomDto> result = await Validator.TestValidateAsync(dto, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Name);
    }
}