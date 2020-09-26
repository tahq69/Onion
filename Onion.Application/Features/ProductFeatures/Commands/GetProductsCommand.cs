using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.DTOs;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    public class GetProductsCommand : PaginationFilter, IRequest<PagedResponse<ICollection<Product>>>
    {
        public string Route { get; set; }

        public class GetProductsHandler : PaginationHandler,
            IRequestHandler<GetProductsCommand, PagedResponse<ICollection<Product>>>
        {
            private readonly IRepository<Product> _products;

            public GetProductsHandler(IRepository<Product> products, IPageUriService pageUri)
                : base(pageUri)
            {
                _products = products;
            }

            public async Task<PagedResponse<ICollection<Product>>> Handle(
                GetProductsCommand request,
                CancellationToken ct)
            {
                var products = await _products.Get(request.PageNumber, request.PageSize);
                var total = await _products.CountAsync();

                return PagedResponse(products, request, total, request.Route);
            }
        }
    }
}