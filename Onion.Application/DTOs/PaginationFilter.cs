namespace Onion.Application.DTOs
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 100 ? 10 : pageSize;
        }

        public PaginationFilter ValidValues() =>
            new PaginationFilter(PageNumber, PageSize);

        public PaginationFilter New(int step) =>
            new PaginationFilter(PageNumber + step, PageSize);
    }
}