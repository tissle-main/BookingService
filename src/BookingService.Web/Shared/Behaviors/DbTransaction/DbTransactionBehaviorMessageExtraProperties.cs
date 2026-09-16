namespace BookingService.Web.Shared.Behaviors.DbTransaction;

/// <summary>Stores transaction options attached to a database message.</summary>
public sealed class DbTransactionBehaviorMessageExtraProperties
{
    public bool BeginDbTransaction { get; set; } = true;
    public bool RollbackOnError { get; set; } = true;
}