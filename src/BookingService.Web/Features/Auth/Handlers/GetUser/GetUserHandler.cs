using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth.Dtos.Users;
using BookingService.Web.Shared.Behaviors.Authorized;

namespace BookingService.Web.Features.Auth.Handlers.GetUser;

public sealed class GetUserHandler : IQueryHandler<GetUserQuery, ErrorOr<UserDto>>
{
    #region Interfaces
    public ValueTask<ErrorOr<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(query.User.ToDto().ToErrorOr());
    }
    #endregion
}