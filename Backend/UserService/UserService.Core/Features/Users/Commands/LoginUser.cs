using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Framework.Endpoints;
using Shared.Kernel;
using UserService.Core.Jwt;
using UserService.Domain.Users;

namespace UserService.Core.Features.Users.Commands;

public record LoginUserCommand(string Email, string Password) : ICommand;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async Task<EndpointResult<string>> (
            [FromBody] LoginUserCommand request,
            [FromServices] LoginUserHandler handler,
            CancellationToken ct) => await handler.Handle(request, ct));
    }
}

public sealed class LoginUserHandler(
    UserManager<User> userManager,
    ITokenProvider tokenProvider) : ICommandHandler<string, LoginUserCommand>
{
    public async Task<Result<string, Error>> Handle(LoginUserCommand command, CancellationToken ct = default)
    {
        User? user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            return Error.NotFound("user.not.found", command.Email);
        }

        var checkPassword = await userManager.CheckPasswordAsync(user, command.Password);
        if (!checkPassword)
        {
            return Error.Authentication("wrong.password", command.Email);
        }

        return tokenProvider.GenerateAccessToken(user, ct);
    }
}