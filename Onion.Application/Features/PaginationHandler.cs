using System;
using System.Collections.Generic;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;
using Onion.Application.Interfaces;

namespace Onion.Application.Features
{
    /// <summary>
    /// Abstract pagination handler.
    /// </summary>
    public abstract class PaginationHandler
    {
        private readonly IApiUriService _apiUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationHandler"/> class.
        /// </summary>
        /// <param name="apiUri">API URI service.</param>
        protected PaginationHandler(IApiUriService apiUri)
        {
            _apiUri = apiUri;
        }

        /// <summary>
        /// Creates paginated response.
        /// </summary>
        /// <param name="data">Paginated data.</param>
        /// <param name="filter">Pagination filter.</param>
        /// <param name="total">Total record count in database.</param>
        /// <param name="route">Resource route for pagination.</param>
        /// <typeparam name="T">Type of the record data.</typeparam>
        /// <returns>Paginated record model.</returns>
        protected PagedResponse<ICollection<T>> PagedResponse<T>(
            ICollection<T> data,
            PaginationFilter filter,
            int total,
            string route)
        {
            var response = new PagedResponse<ICollection<T>>(data, filter.PageNumber, filter.PageSize);
            var totalPages = total / (double)filter.PageSize;
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            var hasNext = filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages;
            response.NextPage = hasNext ? _apiUri.GetPageUri(filter.New(+1), route) : null;

            var hasPrev = filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages;
            response.PreviousPage = hasPrev ? _apiUri.GetPageUri(filter.New(-1), route) : null;

            response.FirstPage = _apiUri.GetPageUri(new PaginationFilter(1, filter.PageSize), route);
            response.LastPage = _apiUri.GetPageUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = total;

            return response;
        }
    }
}