using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel;

namespace Shared.Core.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken ct = default);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}