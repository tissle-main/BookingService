using FluentValidation;
using BookingService.Data.Features.Rooms;

namespace BookingService.Web.Features.Rooms.Dtos;

public sealed class RoomDtoValidator : AbstractValidator<RoomDto>
{
    public RoomDtoValidator()
    {
        base.RuleFor(dto => dto.Name).NotEmpty().MaximumLength(RoomEntityConstants.NameMaxLength);
    }
}