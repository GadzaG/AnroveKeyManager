using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions;

namespace Products.Core;

public static class DependencyInjectionCoreExtensions
{
    public static IServiceCollection AddProductsCore(this IServiceCollection services)
    {
        services.AddHandlers(typeof(DependencyInjectionCoreExtensions).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionCoreExtensions).Assembly);
        return services;
    }
}