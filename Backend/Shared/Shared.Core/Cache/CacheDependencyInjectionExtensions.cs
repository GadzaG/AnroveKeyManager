using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Core.Cache;

public static class CacheDependencyInjectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        // todo добавить сюда потом конфигурацию для редиса
        services.AddStackExchangeRedisCache(setup =>
        {
            setup.Configuration = "redis:6379";
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(30)
            };
        });
        return services;
    }
}