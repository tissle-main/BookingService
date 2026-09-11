using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Data.Features.Auth.RefreshTokens;

namespace BookingService.Web.Features.Auth.Handlers.DeleteUser;

public sealed class DeleteUserHandler(
    AppDbContext thisDbContext,
    UserManager<UserEntity> thisUserManager
) : ICommandHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        UserEntity user = command.User;
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(e => e.UserId == user.Id).ToArrayAsync(cancellationToken);
        thisDbContext.RefreshTokens.RemoveRange(tokens);
        await thisUserManager.DeleteAsync(command.User);
        return Unit.Value;
    }
    #endregion
}