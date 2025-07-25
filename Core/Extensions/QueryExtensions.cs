using Core.Models;

namespace Core.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationRequest request)
    {
        return query
            .Skip(request.Page * request.ItemsPerPage)
            .Take(request.ItemsPerPage);
    }
}