using BookingService.Data;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;
using BookingService.Data.Features.Auth.Roles;
using BookingService.Web.Features.Auth.Options;
using BookingService.Web.Features.Auth.Handlers.GetUser;
using BookingService.Web.Features.Auth.Handlers.GetUsers;
using BookingService.Web.Features.Auth.Handlers.LoginUser;
using BookingService.Web.Features.Auth.Handlers.LogoutUser;
using BookingService.Web.Features.Auth.Handlers.RegisterUser;
using BookingService.Web.Features.Auth.Handlers.GetAdminCredentials;

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
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(IdentityConstants.ApplicationScheme, options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Login";
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = context =>
            {
                if(context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                if(context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        });
        builder.Services.AddAuthorization();
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        if(app.Environment.IsEnvironment(ProfileNames.Test))
        {
            app.AddGetUserEndpoint();
            app.AddGetUsersEndpoint();
            app.AddGetAdminCredentialsEndpoint();
            app.AddRegisterUserEndpoint();
        }
        app.AddLoginUserEndpoint();
        app.AddLogoutUserEndpoint();
    }
}