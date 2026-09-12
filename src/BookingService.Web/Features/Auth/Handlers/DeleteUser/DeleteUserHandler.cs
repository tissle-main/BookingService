using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.DeleteUser;

public sealed class DeleteUserHandler(
    UserManager<UserEntity> thisUserManager
) : ICommandHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await thisUserManager.DeleteAsync(command.User);
        return Unit.Value;
    }
    #endregion
}