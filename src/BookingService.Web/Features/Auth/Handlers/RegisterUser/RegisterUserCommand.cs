using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.RegisterUser;

/// <summary>Requests registration of a new user.</summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's initial password.</param>
public sealed record class RegisterUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;