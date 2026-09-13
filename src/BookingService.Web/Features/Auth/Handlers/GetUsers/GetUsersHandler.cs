using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using BookingService.Web.Features.Auth.Dtos.Users;

namespace BookingService.Web.Features.Auth.Handlers.GetUsers;

public sealed class GetUsersHandler(AppDbContext thisDbContext) : IQueryHandler<GetUsersQuery, ErrorOr<IEnumerable<UserDto>>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<IEnumerable<UserDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        Guid[] ids = query.Ids.Distinct().ToArray();
        if(ids.Length == 0)
        {
            return await thisDbContext.Users.AsNoTracking().ProjectToDto().ToArrayAsync(cancellationToken);
        }

        UserDto[] users = await thisDbContext.Users.AsNoTracking().Where(e => ids.Contains(e.Id)).ProjectToDto().ToArrayAsync(cancellationToken);
        if(ids.Length > users.Length)
        {
            return AuthErrors.UserNotFound();
        }
        return users;
    }
    #endregion
}