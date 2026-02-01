using System.Data;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Shared.Core.Database;
using Shared.Kernel;

namespace Products.Infrastructure.Postgres;

public class TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    : ITransactionScope, IDisposable
{
    private bool _disposed;

    public UnitResult<Error> Commit()
    {
        try
        {
            transaction.Commit();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to commit transaction");
            return Error.Failure("transaction.commit.failed", "Failed to commit transaction");
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            transaction.Rollback();
            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction");
            return UnitResult.Failure(Error.Failure("transaction.rollback.failed", "Failed to rollback transaction"));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Удаляем управляемые ресурсы
            transaction?.Dispose();
        }

        _disposed = true;
    }
}