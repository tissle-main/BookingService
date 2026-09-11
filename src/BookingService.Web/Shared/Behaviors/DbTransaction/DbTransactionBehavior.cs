using ErrorOr;
using Mediator;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingService.Web.Shared.Behaviors.DbTransaction;

public sealed class DbTransactionBehavior<TMessage, TErrorOrValue>(AppDbContext thisDbContext) : IPipelineBehavior<TMessage, TErrorOrValue>
    where TMessage : IDbTransactionBehaviorMessage
    where TErrorOrValue : IErrorOr
{
    #region Interfaces
    public async ValueTask<TErrorOrValue> Handle(TMessage message, MessageHandlerDelegate<TMessage, TErrorOrValue> next, CancellationToken cancellationToken)
    {
        if(!message.BeginDbTransaction)
        {
            return await next(message, cancellationToken);
        }
        return await thisDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async() =>
        {
            await using IDbContextTransaction transaction = await thisDbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                TErrorOrValue errorOrValue = await next(message, cancellationToken);
                if(errorOrValue.IsError && message.RollbackOnError)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                else
                {
                    await transaction.CommitAsync(cancellationToken);
                }
                return errorOrValue;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
    #endregion
}