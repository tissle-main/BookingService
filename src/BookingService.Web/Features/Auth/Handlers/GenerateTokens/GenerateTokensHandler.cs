using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.Extensions.Options;
using BookingService.Web.Features.Auth.Options;
using BookingService.Web.Features.Auth.Services;
using BookingService.Data.Features.Auth.RefreshTokens;
using BookingService.Web.Shared.Behaviors.DbTransaction;
using BookingService.Web.Features.Auth.Dtos.RefreshTokens;
using BookingService.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

namespace BookingService.Web.Features.Auth.Handlers.GenerateTokens;

public sealed class GenerateTokensHandler(
    AppDbContext thisDbContext,
    IMediator thisMediator,
    IOptionsSnapshot<RefreshTokenOptions> tokenOptions,
    IAccessTokenGenerator thisAccessTokenGenerator,
    IRefreshTokenGenerator thisRefreshTokenGenerator
) : ICommandHandler<GenerateTokensCommand, ErrorOr<GenerateTokensResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<GenerateTokensResponse>> Handle(GenerateTokensCommand command, CancellationToken cancellationToken)
    {
        ErrorOr<Unit> result = await thisMediator.Send(new RemoveExpiredRefreshTokensCommand()
        {
            BeginDbTransaction = false
        }, cancellationToken);
        if(result.IsError)
        {
            return result.Errors;
        }

        string refreshToken = await thisRefreshTokenGenerator.GenerateTokenAsync(RefreshTokenEntityConstants.RefreshTokenMaxLength, cancellationToken);
        RefreshTokenEntity entity = new()
        {
            Value = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenOptions.Value.ExpireDays),
            UserId = command.User.Id,
        };
        await thisDbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        string accessToken = await thisAccessTokenGenerator.GenerateTokenAsync(command.User, cancellationToken);
        return new GenerateTokensResponse(accessToken, entity.ToDto());
    }
    #endregion
}