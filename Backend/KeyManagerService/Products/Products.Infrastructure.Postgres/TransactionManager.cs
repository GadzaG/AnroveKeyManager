using System.Transactions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared.Core.Database;
using Shared.Kernel;

namespace Products.Infrastructure.Postgres;

public class TransactionManager(
    ProductsDbContext dbContext,
    ILogger<TransactionManager> logger,
    ILoggerFactory loggerFactory)
    : ITransactionManager
{
    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(ct);
            ILogger<TransactionScope> transactionScopeLogger = loggerFactory.CreateLogger<TransactionScope>();
            TransactionScope transactionScope = new(transaction.GetDbTransaction(), transactionScopeLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to begin transaction");
            return Error.Failure("database", "Failed to begin transaction");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save changes");
            return Error.Failure("database", "Failed to save changes");
        }
    }
}