using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UserService.Infrastructure.Jwt;
using UserService.Web.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("UserService starting");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    string environment = builder.Environment.EnvironmentName;

    builder.Configuration.AddJsonFile($"appsettings.{environment}.json", true, true);

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddConfiguration(builder.Configuration);

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
#pragma warning disable CA5404
                ValidateAudience = false,
#pragma warning restore CA5404
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "UserService",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("464baaae-3428-4795-8ad9-dd3e9ada87d9"))
            };
        });

    builder.Services.AddAuthorization();

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//         .AddJwtBearer(options =>
//         {
//             JwtOptions? jwtOptions = builder.Configuration.GetSection(JwtOptions.SECTION_NAME).Get<JwtOptions>();
//             if (jwtOptions == null)
//                 throw new NotImplementedException();
//
//             options.TokenValidationParameters = new TokenValidationParameters
//             {
//                 ValidateIssuer = true,
//                 ValidateAudience = true,
//                 ValidateLifetime = true,
//                 ValidateIssuerSigningKey = true,
//                 ValidIssuer = jwtOptions?.Issuer,
//                 IssuerSigningKey = new SymmetricSecurityKey(
//                     Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
//                 ClockSkew = TimeSpan.Zero // Убираем задержку проверки времени
//             };
//         });
//
// // Включение авторизации
//     builder.Services.AddAuthorization();
    WebApplication app = builder.Build();

    app.Configure();

    app.UseAuthentication();  // Проверяет токен/куки
    app.UseAuthorization();   // Проверяет права доступа

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UserService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// todo вынести авторизацию и аутентификацию в Shared проект