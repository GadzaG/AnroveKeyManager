using Serilog;
using Shared.Framework.Endpoints;
using Shared.Framework.Middlewares;
using Shared.Framework.Swagger;
using UserService.Infrastructure.Postgres;

namespace UserService.Web.Configuration;

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
            options.SwaggerEndpoint("/openapi/v1.json", "User Service V1");
        });

        app.UseAuthentication();  // Проверяет токен/куки
        app.UseAuthorization();   // Проверяет права доступа

        app.MapEndpoints();
        /*IServiceScope scope = app.Services.CreateScope();
        UserServiceDbContext context = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
        context.Database.EnsureCreated();*/
        return app;
    }
}