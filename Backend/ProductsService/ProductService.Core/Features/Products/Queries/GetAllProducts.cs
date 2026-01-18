using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.Products.Dtos;
using ProductService.Core.Database;
using Shared.Core.Abstractions;
using Shared.Framework.Endpoints;
using Shared.Kernel;

namespace ProductService.Core.Features.Products.Queries;

public enum ProductOrderBy
{
    CREATED_AT,
    UPDATED_AT,
    TITLE
}

// public record GetAllProductsQuery(Guid UserId, int Page, int PageSize) : IQuery;
public record GetAllProductsQuery() : IQuery;

public class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async Task<EndpointResult<List<ProductDto>>> (
            [FromServices] GetAllProductsHandler handler,
            CancellationToken cancellationToken) => await handler.Handle(new GetAllProductsQuery(), cancellationToken));
    }
}

public sealed class GetAllProductsHandler(IProductServiceReadDbContext context) : IQueryHandlerWithResult<List<ProductDto>,  GetAllProductsQuery>
{
    // public async Task<List<Product>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken = default)
    // {
    //     return await context.ProductsQueryable.ToListAsync(cancellationToken);
    // }
    public async Task<Result<List<ProductDto>, Error>> Handle(
        GetAllProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductsQueryable.Select(p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title.Value,
            Description = p.Description.Value
        }).ToListAsync(cancellationToken);
    }
}