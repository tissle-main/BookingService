using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Data.Features.Auth.RefreshTokens;

namespace BookingService.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

//TODO: make it as background service
public sealed class RemoveExpiredRefreshTokensHandler(AppDbContext thisDbContext) : ICommandHandler<RemoveExpiredRefreshTokensCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(RemoveExpiredRefreshTokensCommand command, CancellationToken cancellationToken)
    {
        RefreshTokenEntity[] tokens = await thisDbContext.RefreshTokens.AsNoTracking().Where(e => DateTime.UtcNow > e.ExpiresAt).ToArrayAsync(cancellationToken);
        thisDbContext.RefreshTokens.RemoveRange(tokens);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}