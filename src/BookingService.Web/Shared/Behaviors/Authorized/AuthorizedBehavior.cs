using ErrorOr;
using Mediator;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehavior<TMessage, TErrorOrValue>(
    UserManager<UserEntity> thisUserManager,
    IHttpContextAccessor thisHttpContextAccessor
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
        ClaimsPrincipal principal = thisHttpContextAccessor.HttpContext!.User;
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