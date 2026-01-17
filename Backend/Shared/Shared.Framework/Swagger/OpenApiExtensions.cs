using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace Shared.Framework.Swagger;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiSpec(this IServiceCollection services, string title, string version)
    {
        services.AddOpenApi();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Description = null,
                Version = version,
                TermsOfService = null,
                Contact = new OpenApiContact
                {
                    Name = "anrove",
                    Email = "andrey@vegele.ru"
                },
                License = null,
                Extensions = null
            });
        });

        return services;
    }
}