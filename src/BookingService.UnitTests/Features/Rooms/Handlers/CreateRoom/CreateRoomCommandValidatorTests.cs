using Bogus;
using FluentValidation.TestHelper;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.UnitTests.Features.Rooms.Dtos;
using BookingService.Web.Features.Rooms.Handlers.CreateRoom;

namespace BookingService.UnitTests.Features.Rooms.Handlers.CreateRoom;

public sealed class CreateRoomCommandValidatorTests
{
    private CreateRoomCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance();

        //Act
        TestValidationResult<CreateRoomCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [DependsOn<RoomDtoValidatorTests>(nameof(RoomDtoValidatorTests.Validator_ShouldNotPass_WhenNameIsEmpty))]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        RoomDto room = new Faker<RoomEntity>().ValidInstance().WithEmptyName().Generate().ToDto();
        CreateRoomCommand command = new Faker<CreateRoomCommand>().ValidInstance().WithRoom(room);

        //Act
        TestValidationResult<CreateRoomCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}