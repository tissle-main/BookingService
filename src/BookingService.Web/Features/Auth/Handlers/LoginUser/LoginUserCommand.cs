using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.LoginUser;

/// <summary>Requests authentication for an existing user.</summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public sealed record class LoginUserCommand(string Email, string Password) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<LoginUserResponse>>;