using Products.Domain;

namespace Products.Core.Database;

public interface IProductServiceReadDbContext
{
    IQueryable<Product> ProductsQueryable { get; }
}