using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.CheckLoggedInUser;

public sealed class CheckLoggedInUserHandler : ICommandHandler<CheckLoggedInUserCommand, ErrorOr<bool>>
{
    #region Interfaces
    public ValueTask<ErrorOr<bool>> Handle(CheckLoggedInUserCommand command, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult<ErrorOr<bool>>(
            string.Equals(command.User.Email, command.Email, StringComparison.OrdinalIgnoreCase)
        );
    }
    #endregion
}