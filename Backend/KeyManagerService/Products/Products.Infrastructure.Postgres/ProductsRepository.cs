using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Products.Core.Database;
using Products.Domain;
using Shared.Kernel;

namespace Products.Infrastructure.Postgres;

public class ProductsRepository(
    ProductsDbContext context,
    ILogger<ProductsRepository> logger) : IProductsRepository
{
    public async Task<Result<Guid, Error>> Add(Product product, CancellationToken ct = default)
    {
        try
        {
            await context.Products.AddAsync(product, ct);
            return product.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating product");
            return Error.Failure();
        }
    }

    public async Task<Result<Product, Error>> GetBy(Expression<Func<Product, bool>> predicate, CancellationToken ct = default)
    {
        Product? product = await context.Products.FirstOrDefaultAsync(predicate, ct);
        if (product is null)
            return GeneralErrors.NotFound(null, "product not found");

        return product;
    }
}