using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Core.Features.Products.Queries;
using Shared.Core.Abstractions;

namespace ProductService.Core;

public static class DependencyInjectionCoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddHandlers(typeof(DependencyInjectionCoreExtensions).Assembly);
        services.AddScoped<GetAllProductsHandler>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionCoreExtensions).Assembly);
        return services;
    }
}