using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Core.Jwt;

namespace UserService.Infrastructure.Jwt;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTokenProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SECTION_NAME));
        services.AddScoped<ITokenProvider, JwtTokenProvider>();
        return services;
    }
}