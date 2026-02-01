using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Framework.Auth;
using UserService.Core.Jwt;

namespace UserService.Infrastructure.Jwt;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTokenProvider(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SECTION_NAME));
        services.AddScoped<ITokenProvider, JwtTokenProvider>();
        return services;
    }
}