using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;
using Onion.Application.Interfaces;

namespace Onion.Web.Services
{
    /// <summary>
    /// API URL generator service.
    /// </summary>
    public class ApiUriService : IApiUriService
    {
        private readonly Uri _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUriService"/> class.
        /// </summary>
        /// <param name="baseUri">Application base uri part.</param>
        public ApiUriService(string baseUri)
        {
            _baseUri = new Uri(baseUri);
        }

        /// <inheritdoc />
        public Uri GetPageUri(PaginationFilter filter, string route)
        {
            var uri = GetUri(route).ToString();
            uri = QueryHelpers.AddQueryString(uri, "pageNumber", filter.PageNumber.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", filter.PageSize.ToString());

            return new Uri(uri);
        }

        /// <inheritdoc />
        public Uri GetUri(string route) =>
            new Uri(_baseUri, route);

        /// <inheritdoc />
        public Uri GetUri(string route, params KeyValuePair<string, string>[] queryParams)
        {
            var uri = GetUri(route).ToString();
            foreach (KeyValuePair<string, string> value in queryParams)
            {
                uri = QueryHelpers.AddQueryString(uri, value.Key, value.Value);
            }

            return new Uri(uri);
        }
    }
}