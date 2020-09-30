using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Queries
{
    /// <summary>
    /// Get product record by identifier.
    /// </summary>
    public class GetProductByIdQuery : IRequest<Product?>
    {
        /// <summary>
        /// Gets or sets primary key of the product entity.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Get product record by identifier handler.
        /// </summary>
        public class GetByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
        {
            private readonly IRepository<Product, long> _repository;

            /// <summary>
            /// Initializes a new instance of the <see cref="GetByIdHandler"/> class.
            /// </summary>
            /// <param name="repository">Product entity repository.</param>
            public GetByIdHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

            /// <inheritdoc/>
            public Task<Product?> Handle(GetProductByIdQuery query, CancellationToken ct) =>
                _repository.FirstOrDefault(query.Id);
        }
    }
}