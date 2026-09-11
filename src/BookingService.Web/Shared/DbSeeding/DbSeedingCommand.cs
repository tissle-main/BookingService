using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Shared.DbSeeding;

public sealed record class DbSeedingCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;