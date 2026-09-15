using Bogus;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.CreateRoom;

public static class CreateRoomCommandFaker
{
    extension(Faker<CreateRoomCommand> thisFaker)
    {
        public Faker<CreateRoomCommand> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                RoomDto dto = new Faker<RoomEntity>().ValidInstance().Generate().ToDto();
                return new CreateRoomCommand(dto);
            });
        }
        public Faker<CreateRoomCommand> WithRoom(RoomDto room)
        {
            return thisFaker.RuleFor(c => c.Room, room);
        }
    }
}