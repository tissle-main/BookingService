using ErrorOr;
using Mediator;
using BookingService.Web.Features.Auth;

namespace BookingService.Web.Shared.DbSeeding;

public sealed class DbSeedingHandler(IServiceProvider thisServiceProvider) : ICommandHandler<DbSeedingCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(DbSeedingCommand command, CancellationToken cancellationToken)
    {
        ErrorOr<Unit> errorOrValue = await thisServiceProvider.AddRolesAsync();
        return await errorOrValue.ThenAsync(async(Unit unit) =>
        {
            return await thisServiceProvider.AddRolesAsync();
        });
    }
    #endregion
}