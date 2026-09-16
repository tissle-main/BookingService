using Mediator;

namespace BookingService.Web.Shared.Behaviors.DbTransaction;

/// <summary>Marks whole mediator request to be wrapped into database transaction.</summary>
public interface IDbTransactionBehaviorMessage : IMessage;