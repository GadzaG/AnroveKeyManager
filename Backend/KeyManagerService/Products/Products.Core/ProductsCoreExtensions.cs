using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions;

namespace Products.Core;

public static class ProductsCoreExtensions
{
    public static IServiceCollection AddProductsCore(this IServiceCollection services)
    {
        services.AddHandlers(typeof(ProductsCoreExtensions).Assembly);
        services.AddValidatorsFromAssembly(typeof(ProductsCoreExtensions).Assembly);
        return services;
    }
}