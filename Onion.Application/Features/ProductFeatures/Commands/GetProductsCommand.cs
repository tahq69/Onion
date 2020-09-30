using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.DTOs;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    /// <summary>
    /// Get paginated product command.
    /// </summary>
    public class GetProductsCommand : PaginationFilter, IRequest<PagedResponse<ICollection<Product>>>
    {
        /// <summary>
        /// Gets or sets product resource route.
        /// </summary>
        public string Route { get; set; } = null!;

        /// <summary>
        /// Get paginated product command handler.
        /// </summary>
        public class GetProductsHandler : PaginationHandler,
            IRequestHandler<GetProductsCommand, PagedResponse<ICollection<Product>>>
        {
            private readonly IRepository<Product, long> _products;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetProductsHandler"/> class.
            /// </summary>
            /// <param name="products">Product repository.</param>
            /// <param name="apiUri">API URI service.</param>
            public GetProductsHandler(IRepository<Product, long> products, IApiUriService apiUri)
                : base(apiUri)
            {
                _products = products;
            }

            /// <inheritdoc />
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