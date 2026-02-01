using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Core;
using Products.Infrastructure.Postgres;
using Shared.Framework.Endpoints;

namespace Products.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services,  IConfiguration configuration)
    {
        services.AddProductsCore();
        services.AddProductsInfrastructurePostgres(configuration);
        services.AddEndpoints(typeof(ProductsCoreExtensions).Assembly);
        return services;
    }
}