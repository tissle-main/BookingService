using FluentValidation;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        base.RuleFor(e => e.Room).SetValidator(new RoomDtoValidator());
    }
}