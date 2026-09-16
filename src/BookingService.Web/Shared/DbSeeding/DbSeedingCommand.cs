using ErrorOr;
using Mediator;
using BookingService.Web.Shared.Behaviors.DbTransaction;

namespace BookingService.Web.Shared.DbSeeding;

/// <summary>Requests database seeding for the configured application data.</summary>
public sealed record class DbSeedingCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;