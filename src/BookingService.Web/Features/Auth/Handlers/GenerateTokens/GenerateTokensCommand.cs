using ErrorOr;
using Mediator;
using BookingService.Data.Features.Auth.Users;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensCommand(UserEntity User) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<GenerateTokensResponse>>;