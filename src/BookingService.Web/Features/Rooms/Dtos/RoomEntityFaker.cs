using Bogus;
using BookingService.Data.Features.Rooms;

namespace BookingService.Web.Features.Rooms.Dtos;

public static class RoomEntityFaker
{
    extension(Faker<RoomEntity> thisFaker)
    {
        public Faker<RoomEntity> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                return new RoomEntity()
                {
                    Name = g.Random.String2(RoomEntityConstants.NameMaxLength)
                };
            });
        }
        public Faker<RoomEntity> WithEmptyName()
        {
            return thisFaker.RuleFor(e => e.Name, string.Empty);
        }
        public Faker<RoomEntity> WithTooLargeName()
        {
            return thisFaker.RuleFor(e => e.Name, g => g.Random.String2(RoomEntityConstants.NameMaxLength + 1));
        }
    }
}