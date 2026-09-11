using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Auth.Extensions;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Data.Features.Auth.RefreshTokens;
using BookingService.Web.Shared.Behaviors.DbTransaction;
using BookingService.Web.Features.Auth.Handlers.GenerateTokens;

namespace BookingService.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed class RefreshAccessTokenHandler(
    AppDbContext thisDbContext,
    IHttpContextAccessor thisHttpContextAccessor,
    IMediator thisMediator
) : ICommandHandler<RefreshAccessTokenCommand, ErrorOr<RefreshAccessTokenResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<RefreshAccessTokenResponse>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        if(thisHttpContextAccessor.HttpContext!.GetRefreshToken() is not string refreshToken)
        {
            return Error.Unauthorized();
        }

        RefreshTokenEntity? entity = await thisDbContext.RefreshTokens.AsNoTracking().Include(e => e.User).FirstOrDefaultAsync(
            e => e.Value == refreshToken,
            cancellationToken
        );
        if(entity is null || DateTime.UtcNow > entity.ExpiresAt)
        {
            return Error.Unauthorized();
        }

        thisDbContext.RefreshTokens.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);

        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(entity.User!)
        {
            BeginDbTransaction = false
        }, cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            thisHttpContextAccessor.HttpContext!.AddRefreshToken(tokens.RefreshToken);
            return new RefreshAccessTokenResponse(entity.User!.ToDto(), tokens.AccessToken);
        });
    }
    #endregion
}