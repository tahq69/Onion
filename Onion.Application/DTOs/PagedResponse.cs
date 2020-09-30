using System;

namespace Onion.Application.DTOs
{
    /// <summary>
    /// Paginated record response model.
    /// </summary>
    /// <typeparam name="T">Type of the record data.</typeparam>
    public class PagedResponse<T> : Response<T>
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
        /// Gets or sets first page URL.
        /// </summary>
        public Uri? FirstPage { get; set; }

        /// <summary>
        /// Gets or sets last page URL.
        /// </summary>
        public Uri? LastPage { get; set; }

        /// <summary>
        /// Gets or sets total page count.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets total record count.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets next page URL.
        /// </summary>
        public Uri? NextPage { get; set; }

        /// <summary>
        /// Gets or sets previous page URL.
        /// </summary>
        public Uri? PreviousPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponse{T}"/> class.
        /// </summary>
        /// <param name="data">Paginated record dara.</param>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="pageSize">Record count per page.</param>
        public PagedResponse(T data, int pageNumber, int pageSize)
            : base(data, null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Message = null;
            Succeeded = true;
            Errors = null;
        }
    }
}