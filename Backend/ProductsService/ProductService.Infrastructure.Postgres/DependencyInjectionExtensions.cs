using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductService.Core.Database;

namespace ProductService.Infrastructure.Postgres;

public static class Constants
{
    public const string DATABASE = "Database";
}

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductsRepository, ProductsRepository>();

        services.AddDbContextPool<ProductServiceDbContext>((sp, options) =>
        {
            string? connectionString = configuration.GetConnectionString(Constants.DATABASE);
            IHostEnvironment hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(connectionString);

            if (hostEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseLoggerFactory(loggerFactory);
        });

        services.AddDbContextPool<IProductServiceReadDbContext, ProductServiceDbContext>((sp, options) =>
        {
            string? connectionString = configuration.GetConnectionString(Constants.DATABASE);
            IHostEnvironment hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            options.UseNpgsql(connectionString);

            if (hostEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseLoggerFactory(loggerFactory);
        });

        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}
