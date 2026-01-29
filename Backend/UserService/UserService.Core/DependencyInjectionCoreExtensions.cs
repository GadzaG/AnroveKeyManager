using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Abstractions;

namespace UserService.Core;

public static class DependencyInjectionCoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddHandlers(typeof(DependencyInjectionCoreExtensions).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionCoreExtensions).Assembly);
        return services;
    }
}