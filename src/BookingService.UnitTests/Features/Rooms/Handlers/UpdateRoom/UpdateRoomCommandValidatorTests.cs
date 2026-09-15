using Bogus;
using FluentValidation.TestHelper;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;
using BookingService.UnitTests.Features.Rooms.Dtos;
using BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

namespace BookingService.UnitTests.Features.Rooms.Handlers.UpdateRoom;

public sealed class UpdateRoomCommandValidatorTests
{
    private UpdateRoomCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance();

        //Act
        TestValidationResult<UpdateRoomCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [DependsOn<RoomDtoValidatorTests>(nameof(RoomDtoValidatorTests.Validator_ShouldNotPass_WhenNameIsEmpty))]
    public async ValueTask Validator_ShouldPass_WhenInstanceIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        RoomDto room = new Faker<RoomEntity>().ValidInstance().WithEmptyName().Generate().ToDto();
        UpdateRoomCommand command = new Faker<UpdateRoomCommand>().ValidInstance().WithRoom(room);

        //Act
        TestValidationResult<UpdateRoomCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrors();
    }
}