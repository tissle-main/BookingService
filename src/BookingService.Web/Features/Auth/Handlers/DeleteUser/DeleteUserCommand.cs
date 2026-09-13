using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.Authorized;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Features.Auth.Handlers.DeleteUser;

public sealed record class DeleteUserCommand : IDbTransactionBehaviorMessage, IAuthorizedBehaviorMessage, ICommand<ErrorOr<Unit>>
{
    #region Intefaces
    public string[] AllowedRoles
    {
        get => DeleteUserEndpoint.AllowedRoles;
    }
    #endregion
}