using Bogus;
using ErrorOr;
using Mediator;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Roles;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Features.Auth.Options;
using BookingService.Web.Features.Auth.Dtos.Roles;

namespace BookingService.Web.Features.Auth;

public static class AuthDbSeeder
{
    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddAuthDbSeederProductionProblems()
        {
            return thisBuilder.ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
    extension(IServiceProvider thisServiceProvider)
    {
        public async ValueTask<ErrorOr<Unit>> AddRolesAsync()
        {
            RoleManager<RoleEntity> roleManager = thisServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();

            RoleEntity adminRole = new Faker<RoleEntity>().Admin().Generate();
            if(await roleManager.FindByNameAsync(adminRole.Name!) is null)
            {
                IdentityResult result = await roleManager.CreateAsync(adminRole);
                if(!result.Succeeded)
                {
                    return result.ToErrors().ToList();
                }
            }

            RoleEntity userRole = new Faker<RoleEntity>().User().Generate();
            if(await roleManager.FindByNameAsync(userRole.Name!) is null)
            {
                IdentityResult result = await roleManager.CreateAsync(userRole);
                if(!result.Succeeded)
                {
                    return result.ToErrors().ToList();
                }
            }

            return Unit.Value;
        }
        public async ValueTask<ErrorOr<Unit>> AddAdminAsync()
        {
            UserManager<UserEntity> userManager = thisServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            IOptions<AdminCredentials> adminCredentials = thisServiceProvider.GetRequiredService<IOptions<AdminCredentials>>();

            UserEntity user = new()
            {
                Email = adminCredentials.Value.Email,
                UserName = adminCredentials.Value.Email
            };
            if(await userManager.FindByEmailAsync(user.Email) is null)
            {
                IdentityResult result = await userManager.CreateAsync(user, adminCredentials.Value.Password);
                if(!result.Succeeded)
                {
                    return result.ToErrors().ToList();
                }
            }

            return Unit.Value;
        }
    }  
}