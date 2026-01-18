using ProductService.Domain.Products;

namespace ProductService.Core.Database;

public interface IProductServiceReadDbContext
{
    IQueryable<Product> ProductsQueryable { get; }
}