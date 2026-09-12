using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.LogoutUser;

public sealed record class LogoutUserCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;