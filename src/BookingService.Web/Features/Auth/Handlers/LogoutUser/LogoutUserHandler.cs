using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;

namespace BookingService.Web.Features.Auth.Handlers.LogoutUser;

public sealed class LogoutUserHandler(
    SignInManager<UserEntity> thisSignInManager
) : ICommandHandler<LogoutUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        await thisSignInManager.SignOutAsync();
        return Unit.Value;
    }
    #endregion
}