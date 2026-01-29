using Shared.Framework.Endpoints;
using Shared.Framework.Logging;
using Shared.Framework.Swagger;
using UserService.Core;
using UserService.Infrastructure.Jwt;
using UserService.Infrastructure.Postgres;

namespace UserService.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();

        services
            .AddSerilogLogging(configuration, "UserService")
            .AddOpenApiSpec("UserService", "v1")
            .AddEndpoints(typeof(DependencyInjectionCoreExtensions).Assembly);

        services
            .AddCore()
            .AddInfrastructurePostgres(configuration)
            .AddTokenProvider(configuration);

        return services;
    }
}
