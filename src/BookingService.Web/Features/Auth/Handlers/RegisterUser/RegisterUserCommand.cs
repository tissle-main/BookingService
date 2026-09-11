using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.RegisterUser;

public sealed record class RegisterUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;