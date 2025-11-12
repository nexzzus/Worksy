using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core.Pagination;

namespace PrivateBlog.Web.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> PaginateAsync<T>(this IQueryable<T> queryable, PaginationRequest request)
    {
        return queryable.OrderBy(e => EF.Property<object>(e, "Id"))
            .Take(request.RecordsPerpage);
    }
}