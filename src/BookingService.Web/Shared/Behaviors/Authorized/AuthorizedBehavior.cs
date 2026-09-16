using ErrorOr;
using Mediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookingService.Web.Shared.Behaviors.Authorized;

/// <summary>Verifies that user is authenticated, has an allowed role, and retrieves <see cref="UserEntity"/> from database.</summary>
public sealed class AuthorizedBehavior<TMessage, TErrorOrValue>(
    UserManager<UserEntity> thisUserManager,
    IHttpContextAccessor thisHttpContextAccessor,
    IServiceProvider thisServiceProvider
) : BehaviorBase<TMessage, TErrorOrValue>
    where TMessage : IAuthorizedBehaviorMessage
    where TErrorOrValue : IErrorOr
{
    #region Base
    public override async ValueTask<TErrorOrValue> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TErrorOrValue> next,
        CancellationToken cancellationToken
    )
    {
        ClaimsPrincipal? principal = null;
        if(thisHttpContextAccessor.HttpContext is HttpContext context)
        {
            principal = context.User;
        }
        else if(thisServiceProvider.GetService<AuthenticationStateProvider>() is AuthenticationStateProvider authStateProvider)
        {
            AuthenticationState authState = await authStateProvider.GetAuthenticationStateAsync();
            principal = authState.User;
        }
        
        if(principal is null)
        {
            throw new NullReferenceException("Can`t get ClaimsPrincipal.");
        }
        if(!principal.Identity?.IsAuthenticated ?? true)
        {
            return FromErrors([Error.Unauthorized()]);
        }
        if(await thisUserManager.GetUserAsync(principal) is not UserEntity user)
        {
            return FromErrors([Error.Unauthorized()]);
        }

        string[] allowedRoles = message.AllowedRoles;
        IList<string> roles = await thisUserManager.GetRolesAsync(user);
        if(!roles.Intersect(allowedRoles).Any())
        {
            return FromErrors([Error.Forbidden()]);
        }

        message.User = user;
        message.Roles = roles;
        return await next(message, cancellationToken);
    }
    #endregion
}