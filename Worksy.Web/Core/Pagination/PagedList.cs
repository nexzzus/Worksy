using Microsoft.EntityFrameworkCore;
using Worksy.Web.Core.Extensions;

namespace Worksy.Web.Core.Pagination;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get;  set; }
    public int RecordsPerPage { get;  set; }
    public int TotalCount { get;  set; }

    public PagedList()
    {
        
    }

    public PagedList(List<T> items, int count, int pageNumber, int recordsPerPage)
    {
        TotalCount = count;
        RecordsPerPage = recordsPerPage;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)recordsPerPage);
        AddRange(items);    
    }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> queryable, PaginationRequest request)
    {
        int count = await queryable.CountAsync();

        List<T> items = await queryable.PaginateAsync<T>(request)
            .ToListAsync();
        
        return new PagedList<T>(items, count, request.Page, request.RecordsPerpage);
    }
}