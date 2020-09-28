using System;
using Onion.Application.DTOs;

namespace Onion.Application.Interfaces
{
    public interface IPageUriService
    {
        public Uri GetUri(PaginationFilter filter, string route);
    }
}