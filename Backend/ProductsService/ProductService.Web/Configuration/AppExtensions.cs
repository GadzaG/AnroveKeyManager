using Serilog;
using Shared.Framework.Endpoints;
using Shared.Framework.Middlewares;

namespace ProductService.Web.Configuration;

public static class AppExtensions
{
    public static IApplicationBuilder Configure(this WebApplication app)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:3001",
                    "http://localhost",
                    "http://frontend:3000")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });

        app.UseExceptionMiddleware();
        app.UseRequestCorrelationId();
        app.UseSerilogRequestLogging();

        app.MapOpenApi();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Products Service V1");
        });

        app.MapEndpoints();

        return app;
    }
}
