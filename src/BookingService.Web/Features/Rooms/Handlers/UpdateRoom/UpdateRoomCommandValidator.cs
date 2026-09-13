using FluentValidation;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

public sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        base.RuleFor(e => e.Room).SetValidator(new RoomDtoValidator());
    }
}