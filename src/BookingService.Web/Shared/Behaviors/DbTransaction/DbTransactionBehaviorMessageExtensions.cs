using System.Runtime.CompilerServices;

namespace BookingService.Web.Shared.Behaviors.DbTransaction;

public static class DbTransactionBehaviorMessageExtensions
{
    private static ConditionalWeakTable<IDbTransactionBehaviorMessage, DbTransactionBehaviorMessageExtraProperties> ExtraPropertiesTable { get; } = [];

    extension(IDbTransactionBehaviorMessage thisMessage)
    {
        public bool BeginDbTransaction
        {
            get => thisMessage.GetExtraProperties().BeginDbTransaction;
            set
            {
                thisMessage.GetExtraProperties().BeginDbTransaction = value;
            }
        }
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