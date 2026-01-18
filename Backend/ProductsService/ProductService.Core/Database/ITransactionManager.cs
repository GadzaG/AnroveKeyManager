using System.Data;
using CSharpFunctionalExtensions;
using Shared.Kernel;

namespace ProductService.Core.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken ct = default);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}