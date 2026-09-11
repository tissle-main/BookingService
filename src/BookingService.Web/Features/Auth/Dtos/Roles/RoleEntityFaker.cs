using Bogus;
using BookingService.Data.Features.Auth.Roles;

namespace BookingService.Web.Features.Auth.Dtos.Roles;

public static class RoleEntityFaker
{
    extension(Faker<RoleEntity> thisFaker)
    {
        public Faker<RoleEntity> Admin()
        {
            return thisFaker.CustomInstantiator(g => new RoleEntity()
            {
                Name = AuthRoles.Admin
            });
        }
        public Faker<RoleEntity> User()
        {
            return thisFaker.CustomInstantiator(g => new RoleEntity()
            {
                Name = AuthRoles.User
            });
        }
    }
}