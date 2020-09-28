using System;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;
using Onion.Application.Interfaces;

namespace Onion.Web.Services
{
    /// <summary>
    /// Pagination URI generator service.
    /// </summary>
    public class PageUriService : IPageUriService
    {
        private readonly string _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageUriService"/> class.
        /// </summary>
        /// <param name="baseUri">Application base uri part.</param>
        public PageUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        /// <inheritdoc cref="IPageUriService.GetUri(PaginationFilter, string)"/>
        public Uri GetUri(PaginationFilter filter, string route)
        {
            var uri = new Uri(string.Concat(_baseUri, route)).ToString();
            uri = QueryHelpers.AddQueryString(uri, "pageNumber", filter.PageNumber.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", filter.PageSize.ToString());

            return new Uri(uri);
        }
    }
}