namespace Worksy.Web.Core.Pagination;

public class PaginationRequest
{
    private int _page = 1;
    private int _recordsPerpage = 15;
    private const int MAX_RECORDS_PER_PAGE = 50;
    
    public string? Filter { get; set; }

    public int Page
    {
        get => _page;
        set => _page = value > 0 ? value : _page;
    }

    public int RecordsPerpage
    {
        get => _recordsPerpage;
        set => _recordsPerpage = value <= MAX_RECORDS_PER_PAGE ? value : MAX_RECORDS_PER_PAGE;
    }
}