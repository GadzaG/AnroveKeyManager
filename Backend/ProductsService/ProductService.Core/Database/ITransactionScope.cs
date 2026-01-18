using CSharpFunctionalExtensions;
using Shared.Kernel;

namespace ProductService.Core.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}