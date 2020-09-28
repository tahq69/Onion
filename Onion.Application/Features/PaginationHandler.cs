using System;
using System.Collections.Generic;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;
using Onion.Application.Interfaces;

namespace Onion.Application.Features
{
    public abstract class PaginationHandler
    {
        private readonly IPageUriService _pageUri;

        protected PaginationHandler(IPageUriService pageUri)
        {
            _pageUri = pageUri;
        }

        protected PagedResponse<ICollection<T>> PagedResponse<T>(
            ICollection<T> data,
            PaginationFilter filter,
            int total,
            string route)
        {
            var response = new PagedResponse<ICollection<T>>(data, filter.PageNumber, filter.PageSize);
            var totalPages = ((double) total / (double) filter.PageSize);
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            
            var hasNext = filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages;
            response.NextPage = hasNext ? _pageUri.GetUri(filter.New(+1), route) : null;
            
            var hasPrev = filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages;
            response.PreviousPage = hasPrev ? _pageUri.GetUri(filter.New(-1), route) : null;
            
            response.FirstPage = _pageUri.GetUri(new PaginationFilter(1, filter.PageSize), route);
            response.LastPage = _pageUri.GetUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = total;

            return response;
        }
    }
}