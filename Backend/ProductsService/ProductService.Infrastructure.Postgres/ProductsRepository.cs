using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Core.Database;
using ProductService.Domain.Products;
using Shared.Kernel;

namespace ProductService.Infrastructure.Postgres;

public class ProductsRepository(
    ProductServiceDbContext context,
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

        // try
        // {
        //     await _dbContext.SaveChangesAsync(cancellationToken);
        //
        //     return lesson.Id;
        // }
        // catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        // {
        //     if (pgEx is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
        //         && pgEx.ConstraintName.Contains(Index.TITLE, StringComparison.InvariantCultureIgnoreCase))
        //     {
        //         return EducationErrors.TitleConflict(lesson.Title.Value);
        //     }
        //
        //     _logger.LogError(ex, "Database update error while creating lesson with title {Title}", lesson.Title.Value);
        //
        //     return EducationErrors.DatabaseError();
        // }
        // catch (OperationCanceledException ex)
        // {
        //     _logger.LogError(ex, "Operation was cancelled while creating lesson with title {Title}", lesson.Title.Value);
        //     return EducationErrors.OperationCancelled();
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Unexpected error while creating lesson with title {Title}", lesson.Title.Value);
        //
        //     return EducationErrors.DatabaseError();
        // }
    }

    public async Task<Result<Product, Error>> GetBy(Expression<Func<Product, bool>> predicate, CancellationToken ct = default)
    {
        Product? product = await context.Products.FirstOrDefaultAsync(predicate, ct);
        if (product is null)
            return GeneralErrors.NotFound(null, "product not found");

        return product;
    }
}