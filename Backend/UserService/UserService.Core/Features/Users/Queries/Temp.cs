using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                    return Results.Unauthorized();

                await Task.Delay(100);
                return Results.Ok("bruh");

                // var userId = Guid.Parse(userIdClaim.Value);
                //
                // var userEntity = await manager.FindByIdAsync(userIdClaim.Value);
                //
                // if (userEntity is null)
                //     return Results.NotFound();
                //
                // return Results.Ok(new
                // {
                //     userEntity.Id,
                //     userEntity.Email,
                // });
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