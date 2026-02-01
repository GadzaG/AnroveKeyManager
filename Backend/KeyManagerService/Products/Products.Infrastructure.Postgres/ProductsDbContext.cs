using Microsoft.EntityFrameworkCore;
using Products.Core.Database;
using Products.Domain;

namespace Products.Infrastructure.Postgres;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options)
    : DbContext(options), IProductsReadDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public IQueryable<Product> ProductsQueryable => Products.AsQueryable().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("products");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
    }
}