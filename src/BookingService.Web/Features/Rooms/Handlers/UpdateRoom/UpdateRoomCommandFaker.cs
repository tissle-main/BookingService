using Bogus;
using BookingService.Data.Features.Rooms;
using BookingService.Web.Features.Rooms.Dtos;

namespace BookingService.Web.Features.Rooms.Handlers.UpdateRoom;

public static class UpdateRoomCommandFaker
{
    extension(Faker<UpdateRoomCommand> thisFaker)
    {
        public Faker<UpdateRoomCommand> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                RoomDto dto = new Faker<RoomEntity>().ValidInstance().Generate().ToDto();
                return new UpdateRoomCommand(dto);
            });
        }
        public Faker<UpdateRoomCommand> WithRoom(RoomDto room)
        {
            return thisFaker.RuleFor(c => c.Room, room);
        }
    }
}