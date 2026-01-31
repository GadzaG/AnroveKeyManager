using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Products.Domain;
using Shared.Kernel;

namespace Products.Core.Database;

public interface IProductsRepository
{
    Task<Result<Guid, Error>> Add(Product product, CancellationToken ct = default);

    Task<Result<Product, Error>> GetBy(
        Expression<Func<Product, bool>> predicate,
        CancellationToken ct = default);
}