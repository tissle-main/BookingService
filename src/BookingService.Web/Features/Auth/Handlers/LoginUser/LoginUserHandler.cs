using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Features.Auth.Dtos.Users;
using LoginResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace BookingService.Web.Features.Auth.Handlers.LoginUser;

/// <summary>Authenticates users and creates their Identity session.</summary>
public sealed class LoginUserHandler(
    SignInManager<UserEntity> thisSignInManager
) : ICommandHandler<LoginUserCommand, ErrorOr<LoginUserResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisSignInManager.UserManager.FindByEmailAsync(command.Email) is not UserEntity user)
        {
            return AuthErrors.UserNotFound();
        }

        LoginResult result = await thisSignInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToError();
        }

        await thisSignInManager.SignInAsync(user, isPersistent: false);
        return new LoginUserResponse(user.ToDto());
    }
    #endregion
}