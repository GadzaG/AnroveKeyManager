using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel;
using UserService.Core.Jwt;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Jwt;

public static class CustomClaims
{
    public const string ID = "Id";

    public const string ROLE = "Role";

    public const string PERMISSION = "Permission";
}

public class JwtOptions
{
    public const string SECTION_NAME = "JwtOptions";

    public string SecretKey { get; init; } = string.Empty;

    public int TokenLifeTimeInMinutes { get; init; } = 5;

    public string Issuer { get; init; } = string.Empty;
}

public class JwtTokenProvider(IOptions<JwtOptions> options) : ITokenProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public Result<string, Error> GenerateAccessToken(User user, CancellationToken ct = default)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()), };

        var ssk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var signingCredentials = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenLifeTimeInMinutes),
            signingCredentials: signingCredentials);

        string? stringToken = new JwtSecurityTokenHandler().WriteToken(token);
        return stringToken;
    }

    public Result<string, Error> GenerateRefreshToken(User user, CancellationToken ct = default)
    {
        // Реализуйте аналогично, но с длинным expiration (e.g. 30 дней) и только ID claim
        throw new NotImplementedException();
    }
}