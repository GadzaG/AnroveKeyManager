using Products.Domain;

namespace Products.Core.Database;

public interface IProductsReadDbContext
{
    IQueryable<Product> ProductsQueryable { get; }
}