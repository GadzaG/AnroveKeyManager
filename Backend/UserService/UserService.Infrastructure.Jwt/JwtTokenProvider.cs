using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Framework.Auth;
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

public class JwtTokenProvider(IOptions<AuthOptions> options) : ITokenProvider
{
    private readonly AuthOptions _authOptions = options.Value;

    public Result<string, Error> GenerateAccessToken(User user, CancellationToken ct = default)
    {
        var claims = new List<Claim> { new("Id", user.Id.ToString()), };

        var ssk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.SecretKey));
        var signingCredentials = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            claims: claims,
            audience: _authOptions.Audience,
            expires: DateTime.UtcNow.AddMinutes(_authOptions.TokenLifeTimeInMinutes),
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