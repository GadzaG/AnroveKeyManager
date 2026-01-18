using Microsoft.EntityFrameworkCore;
using ProductService.Core.Database;
using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Postgres;

public class ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options)
    : DbContext(options), IProductServiceReadDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public IQueryable<Product> ProductsQueryable => Products.AsQueryable().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductServiceDbContext).Assembly);
    }
}