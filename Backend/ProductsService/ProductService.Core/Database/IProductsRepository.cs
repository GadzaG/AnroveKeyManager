using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using ProductService.Domain.Products;
using Shared.Kernel;

namespace ProductService.Core.Database;

public interface IProductsRepository
{
    Task<Result<Guid, Error>> Add(Product product, CancellationToken ct = default);

    Task<Result<Product, Error>> GetBy(
        Expression<Func<Product, bool>> predicate,
        CancellationToken ct = default);
}