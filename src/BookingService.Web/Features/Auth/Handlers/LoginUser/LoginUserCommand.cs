using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<LoginUserResponse>>;