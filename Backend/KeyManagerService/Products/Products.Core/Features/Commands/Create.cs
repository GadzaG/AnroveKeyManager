using System.Security.Claims;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Products.Core.Database;
using Products.Domain;
using Products.Domain.ValueObjects;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Core.Validation;
using Shared.Framework.Endpoints;
using Shared.Kernel;

namespace Products.Core.Features.Commands;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithError(GeneralErrors.ValueIsRequired(nameof(CreateProductCommand.UserId)));

        RuleFor(x => x.Title)
            .MustBeValueObject(Title.Create);

        RuleFor(x => x.Description)
            .MustBeValueObject(Description.Create);
    }
}

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async Task<EndpointResult<Guid>> (
            [FromBody] CreateProductRequest request,
            [FromServices] CreateProductHandler handler,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            // Получаем Id пользователя из claims
            Claim? userIdClaim = user.FindFirst("Id");

            if (userIdClaim is null)
                return new EndpointResult<Guid>(Error.Authorization("auth.error", "user_unauthorized"));

            var userId = Guid.Parse(userIdClaim.Value);

            var command = new CreateProductCommand(userId, request.Title, request.Description);

            return await handler.Handle(command, cancellationToken);

        }).RequireAuthorization();
    }
}

public record CreateProductRequest(string Title, string Description);

public record CreateProductCommand(Guid UserId, string Title, string Description) : ICommand;

public sealed class CreateProductHandler(
    IProductsRepository productsRepository,
    ITransactionManager transactionManager,
    IValidator<CreateProductCommand> validator,
    ILogger<CreateProductHandler> logger) : ICommandHandler<Guid, CreateProductCommand>
{
    public async Task<Result<Guid, Error>> Handle(
        CreateProductCommand command,
        CancellationToken ct = default)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation Failed");
            return validationResult.ToError();
        }

        Title title = Title.Create(command.Title).Value;
        Description description = Description.Create(command.Description).Value;

        Result<Product, Error> createProduct = Product.Create(Guid.NewGuid(), Guid.Empty, title, description); // todo вот тут я изменил временно userId

        if (createProduct.IsFailure)
        {
            return createProduct.Error;
        }

        Product product = createProduct.Value;
        Result<Guid, Error> create = await productsRepository.Add(product, ct);
        if (create.IsFailure)
        {
            return create.Error;
        }

        UnitResult<Error> saveChanges = await transactionManager.SaveChangesAsync(ct);
        if (saveChanges.IsFailure)
        {
            return saveChanges.Error;
        }

        return product.Id;
    }
}