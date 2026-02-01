using Products.Core;
using Products.DependencyInjection;
using Shared.Framework.Auth;
using Shared.Framework.Endpoints;
using Shared.Framework.Logging;
using Shared.Framework.Swagger;

namespace KeyManager.Web.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();

        services
            .AddAuth(configuration)
            .AddSerilogLogging(configuration, "KeyManagerService")
            .AddOpenApiSpec("KeyManagerService", "v1");

        services.AddProducts(configuration);

        return services;
    }
}

/*
[]


*/