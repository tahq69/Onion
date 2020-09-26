using System;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;

namespace Onion.Web.Services
{
    public class PageUriService : IPageUriService
    {
        private readonly string _baseUri;

        public PageUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetUri(PaginationFilter filter, string route)
        {
            var uri = new Uri(string.Concat(_baseUri, route)).ToString();
            uri = QueryHelpers.AddQueryString(uri, "pageNumber", filter.PageNumber.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", filter.PageSize.ToString());

            return new Uri(uri);
        }
    }
}