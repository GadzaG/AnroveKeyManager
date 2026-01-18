using ProductService.Core;
using ProductService.Infrastructure.Postgres;
using Shared.Framework.Endpoints;
using Shared.Framework.Logging;
using Shared.Framework.Swagger;

namespace ProductService.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();

        services
            .AddSerilogLogging(configuration, "ProductService")
            .AddOpenApiSpec("ProductService", "v1")
            .AddEndpoints(typeof(DependencyInjectionCoreExtensions).Assembly);

        services
            .AddCore()
            .AddInfrastructurePostgres(configuration);
        return services;
    }
}
