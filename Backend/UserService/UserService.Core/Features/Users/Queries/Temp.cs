using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.Core.Abstractions;
using Shared.Framework.Endpoints;
using Shared.Kernel;
using UserService.Domain.Users;

namespace UserService.Core.Features.Users.Queries;

public record TempQuery(Guid Id) : IQuery;

public sealed class TempEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // app.MapPost("/temp", async Task<EndpointResult<string>> (
        //         [FromServices] TempHandler handler,
        //         HttpContext context, // ← Добавляем HttpContext
        //         CancellationToken ct = default) =>
        //     {
        //         if (!context.User.Identity?.IsAuthenticated ?? true)
        //             return new EndpointResult<string>(Error.Authorization("unuthorize", "пошел нахуй"));
        //
        //         // 2. Получаем ID пользователя из claims
        //         var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        //         if (userIdClaim is null)
        //         {
        //             return new EndpointResult<string>(Error.Authorization("unuthorize", "пошел нахуй"));
        //         }
        //
        //         var userId = Guid.Parse(userIdClaim.Value);
        //
        //         // 3. Создаём запрос с ID пользователя
        //         var query = new TempQuery(userId);
        //
        //         // 4. Передаём в обработчик
        //         return await handler.Handle(query, ct);
        //     })
        //     .RequireAuthorization(); // Защищаем эндпоинт
        app.MapGet("/get-user-info", async (
                ClaimsPrincipal user,
                [FromServices] UserManager<User> manager) =>
            {
                var userIdClaim = user.FindFirst("Id");

                if (userIdClaim is null)
                    return Results.Unauthorized();

                await Task.Delay(100);
                return Results.Ok("bruh");

                // var userId = Guid.Parse(userIdClaim.Value);
                /*User? userEntity = await manager.FindByIdAsync(userId.ToString());

                if (userEntity is null)
                    return Results.NotFound();

                return Results.Ok(new
                {
                    userEntity.Id,
                    userEntity.Email,
                });*/
            })
            .RequireAuthorization();
    }
}

public sealed class TempHandler(UserManager<User> userManager, ILogger<TempHandler> logger) : IQueryHandlerWithResult<string, TempQuery>
{
    public async Task<Result<string, Error>> Handle(TempQuery query, CancellationToken ct = default)
    {
        logger.LogInformation("TempHandler активирован");
        var user = await userManager.FindByIdAsync(query.Id.ToString());
        if (user is null)
        {
            return Error.NotFound("not.found", "user");
        }

        logger.LogInformation("привет");
        return string.IsNullOrWhiteSpace(user.Email) ? "пусто" : user.Email;
    }
}

public static class ClaimBindingSource
{
    public static readonly BindingSource Claim = new(
        id: "Claim", // уникальный ID
        displayName: "Claim", // имя для отладки
        isGreedy: false,
        isFromRequest: true);
}

public sealed class ClaimValueProvider(BindingSource bindingSource, ClaimsPrincipal user)
    : BindingSourceValueProvider(bindingSource)
{
    public override bool ContainsPrefix(string prefix)
        => user.HasClaim(c => c.Type == prefix);

    public override ValueProviderResult GetValue(string key)
    {
        string? raw = user.FindFirst(key)?.Value;
        if (raw is null)
            return ValueProviderResult.None;

        // если модель Guid или Nullable<Guid> — вернём нормализованную строку
        return Guid.TryParse(raw, out var guid)
            ? new ValueProviderResult(guid.ToString())
            : new ValueProviderResult(raw);
    }
}

public sealed class ClaimValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        var user = context.ActionContext.HttpContext.User;
        context.ValueProviders.Add(
            new ClaimValueProvider(ClaimBindingSource.Claim, user));

        return Task.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromClaimsAttribute(string name) : Attribute, IBindingSourceMetadata, IModelNameProvider
{
    public BindingSource BindingSource => ClaimBindingSource.Claim;

    // тип claim’а
    public string Name { get; } = name;
}