using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Products.Contracts.Dto;
using Products.Core.Database;
using Products.Domain;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Framework.Endpoints;
using Shared.Kernel;

namespace Products.Core.Features.Queries;

public enum ProductOrderBy
{
    CREATED_AT,
    UPDATED_AT,
    TITLE
}

public record GetAllProductsQuery : IQuery
{
    public Guid UserId { get; init; } = Guid.Empty;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public ProductOrderBy? OrderBy { get; init; } = ProductOrderBy.CREATED_AT;
}

public record GetAllProductsRequest(int Page = 1, int PageSize = 20, ProductOrderBy? OrderBy = null);

public class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/products",
            async Task<EndpointResult<PagedList<ProductDto>>> (
                ClaimsPrincipal user,
                [FromServices] GetAllProductsHandler handler,
                CancellationToken ct = default) =>
            {
                Claim? userIdClaim = user.FindFirst("Id");

                if (userIdClaim is null)
                    return new EndpointResult<PagedList<ProductDto>>(Error.Authorization("auth.error", "user_unauthorized"));

                var userId = Guid.Parse(userIdClaim.Value);
                var query = new GetAllProductsQuery { UserId = userId, OrderBy = ProductOrderBy.CREATED_AT, Page = 1, PageSize = 20 };
                return await handler.Handle(query, ct);
            });
    }
}

public sealed class GetAllProductsHandler(
    IProductsReadDbContext context)
    : IQueryHandlerWithResult<PagedList<ProductDto>, GetAllProductsQuery>
{
    public async Task<Result<PagedList<ProductDto>, Error>> Handle(
        GetAllProductsQuery query,
        CancellationToken ct = default)
    {
        IQueryable<Product> response = context.ProductsQueryable;

        if (query.OrderBy.HasValue)
        {
            response = query.OrderBy switch
            {
                ProductOrderBy.CREATED_AT => response.OrderBy(p => p.CreatedAt),
                ProductOrderBy.TITLE => response.OrderBy(p => p.Title),
                ProductOrderBy.UPDATED_AT => response.OrderBy(p => p.UpdatedAt),
                _ => response
            };
        }

        IQueryable<ProductDto> resultQuery = response.Select(p => new ProductDto
        {
            Id = p.Id, Title = p.Title.Value, Description = p.Description.Value
        });

        return await resultQuery.ToPagedList(query.Page, query.PageSize, ct);
    }
}