using System;
using System.Collections.Generic;
using Onion.Application.DTOs;

namespace Onion.Application.Interfaces
{
    /// <summary>
    /// API URL service contract.
    /// </summary>
    public interface IApiUriService
    {
        /// <summary>
        /// Get URL to the pagination page.
        /// </summary>
        /// <param name="filter">Pagination filter.</param>
        /// <param name="route">Route of the pagination resource.</param>
        /// <returns>Pagination resource URL.</returns>
        Uri GetPageUri(PaginationFilter filter, string route);

        /// <summary>
        /// Create URL to resource.
        /// </summary>
        /// <param name="route">Route of the resource.</param>
        /// <returns>Resource URL.</returns>
        Uri GetUri(string route);

        /// <summary>
        /// Create URL to resource with query string parameters.
        /// </summary>
        /// <param name="route">Route of the resource.</param>
        /// <param name="queryParams">Query parameters.</param>
        /// <returns>Resource URL.</returns>
        Uri GetUri(string route, params KeyValuePair<string, string>[] queryParams);
    }
}