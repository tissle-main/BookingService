using BookingService.Data;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Features.Auth.Roles;
using Microsoft.AspNetCore.Authentication.Cookies;
using BookingService.Web.Features.Auth.Options;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Auth.Handlers.DeleteUser;
using BookingService.Web.Features.Auth.Handlers.RegisterUser;

namespace BookingService.Web.Features.Auth;

public sealed class AuthFeatureProvider : FeatureProvider
{
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<AdminCredentials>().BindConfiguration(AdminCredentials.SectionName);

        builder.Services.AddIdentityCore<UserEntity>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = AuthConstants.PasswordMinLength;
            options.Password.RequireNonAlphanumeric = false;
        }).AddRoles<RoleEntity>().AddSignInManager().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();      
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/login";
                options.SlidingExpiration = true;
            });
        builder.Services.AddAuthorization();
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.AddRegisterUserEndpoint();
        app.AddLoginUserEndpoint();
        app.AddDeleteUserEndpoint();
    }
}