using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared.Core.Database;

public static class QueriesExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int totalCount = await source.CountAsync(cancellationToken);

        List<T> items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PagedList<T>
        {
            Items = items, PageSize = pageSize, Page = page, TotalCount = totalCount,
        };
    }

    public static async Task<PagedList<TResult>> ToPagedList<T, TResult>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        Func<T, TResult> mapper,
        CancellationToken cancellationToken = default)
    {
        int totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PagedList<TResult>
        {
            Items = items.Select(mapper).ToList(), PageSize = pageSize, Page = page, TotalCount = totalCount,
        };
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}