namespace Onion.Application.DTOs
{
    /// <summary>
    /// Pagination filter model.
    /// </summary>
    public class PaginationFilter
    {
        /// <summary>
        /// Gets or sets current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets record count per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationFilter"/> class.
        /// </summary>
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationFilter"/> class.
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="pageSize">Record count per page.</param>
        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 100 ? 10 : pageSize;
        }

        /// <summary>
        /// Create new instance with valid values.
        /// </summary>
        /// <returns>Recreated object with valid pagination filter values.</returns>
        public PaginationFilter ValidValues() =>
            new PaginationFilter(PageNumber, PageSize);

        /// <summary>
        /// Creates new object with updated current page.
        /// </summary>
        /// <param name="step">Delta to update current page.</param>
        /// <returns>Recreated object with update current page.</returns>
        public PaginationFilter New(int step) =>
            new PaginationFilter(PageNumber + step, PageSize);
    }
}