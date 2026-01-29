using CSharpFunctionalExtensions;
using Shared.Kernel;
using UserService.Domain.Users;

namespace UserService.Core.Jwt;

public interface ITokenProvider
{
    Result<string, Error> GenerateAccessToken(User user, CancellationToken ct = default);

    Result<string, Error> GenerateRefreshToken(User user, CancellationToken ct = default);
}