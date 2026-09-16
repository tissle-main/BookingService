using System.Runtime.CompilerServices;

namespace BookingService.Web.Shared.Behaviors.DbTransaction;

/// <summary>
/// Provides extension properties for <see cref="IDbTransactionBehaviorMessage" />
/// </summary>
public static class DbTransactionBehaviorMessageExtensions
{
    private static ConditionalWeakTable<IDbTransactionBehaviorMessage, DbTransactionBehaviorMessageExtraProperties> ExtraPropertiesTable { get; } = [];

    extension(IDbTransactionBehaviorMessage thisMessage)
    {
        /// <summary>Gets or sets whether a transaction should be started.</summary>
        public bool BeginDbTransaction
        {
            get => thisMessage.GetExtraProperties().BeginDbTransaction;
            set
            {
                thisMessage.GetExtraProperties().BeginDbTransaction = value;
            }
        }

        /// <summary>Gets or sets whether errors should roll back the transaction.</summary>
        public bool RollbackOnError
        {
            get => thisMessage.GetExtraProperties().RollbackOnError;
            set
            {
                thisMessage.GetExtraProperties().RollbackOnError = value;
            }
        }

        private DbTransactionBehaviorMessageExtraProperties GetExtraProperties()
        {
            return ExtraPropertiesTable.GetOrAdd(thisMessage, static _ => new DbTransactionBehaviorMessageExtraProperties());
        }
    }
}