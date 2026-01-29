using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Core.Validation;
using Shared.Framework.Endpoints;
using Shared.Kernel;
using UserService.Domain.Users;

namespace UserService.Core.Features.Users.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator() // todo тут заменить withMessage на свой with error
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithError(Error.Validation("validation", "Email is required"));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}

public record RegisterUserCommand(string Email, string Password) : ICommand;

public sealed class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/registration", async Task<EndpointResult<Guid>> (
            [FromBody] RegisterUserCommand request,
            [FromServices] RegisterUserHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public class RegisterUserHandler(
    UserManager<User> userManager,
    IValidator<RegisterUserCommand> validator,
    ILogger<RegisterUserHandler> logger) : ICommandHandler<Guid, RegisterUserCommand>
{
    public async Task<Result<Guid, Error>> Handle(
        RegisterUserCommand command,
        CancellationToken ct = default)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation Failed");
            return validationResult.ToError();
        }

        var user = new User { Email = command.Email, UserName = command.Email };
        IdentityResult createUser = await userManager.CreateAsync(user, command.Password);
        if (createUser.Succeeded)
            return user.Id;

        var errorMessages = createUser.Errors.Select(e => new ErrorMessage(e.Code, e.Description)).ToList();

        var createUserError = Error.Authentication(errorMessages);
        return createUserError;

    }
}