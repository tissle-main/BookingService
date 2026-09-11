using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

public sealed record class RemoveExpiredRefreshTokensCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;