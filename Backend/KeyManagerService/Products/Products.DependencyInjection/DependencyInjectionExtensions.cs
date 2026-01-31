using Microsoft.Extensions.DependencyInjection;
using Products.Core;

namespace Products.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services)
    {
        services.AddProductsCore();
        return services;
    }
}