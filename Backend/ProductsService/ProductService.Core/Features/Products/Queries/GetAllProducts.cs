using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.Products.Dtos;
using ProductService.Core.Database;
using ProductService.Domain.Products;
using Shared.Core.Abstractions;
using Shared.Core.Database;
using Shared.Framework.Endpoints;
using Shared.Kernel;

namespace ProductService.Core.Features.Products.Queries;

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

public class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/products",
            async Task<EndpointResult<PagedList<ProductDto>>> (
                    [FromServices] GetAllProductsHandler handler,
                    CancellationToken cancellationToken) =>
                await handler.Handle(new GetAllProductsQuery(), cancellationToken));
    }
}

public sealed class GetAllProductsHandler(
    IProductServiceReadDbContext context)
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