using System;
using Onion.Application.DTOs;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    public interface IPageUriService
    {
        public Uri GetUri(PaginationFilter filter, string route);
    }
}