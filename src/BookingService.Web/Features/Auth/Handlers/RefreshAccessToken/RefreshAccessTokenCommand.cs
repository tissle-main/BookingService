using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<RefreshAccessTokenResponse>>;