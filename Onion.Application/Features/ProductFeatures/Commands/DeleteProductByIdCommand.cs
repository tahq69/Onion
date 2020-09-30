using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    /// <summary>
    /// Delete product record by identifier command.
    /// </summary>
    public class DeleteProductByIdCommand : IRequest<long>
    {
        /// <summary>
        /// Gets or sets product identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Delete product record by identifier command handler.
        /// </summary>
        public class DeleteProductByIdHandler : IRequestHandler<DeleteProductByIdCommand, long>
        {
            private readonly IRepository<Product, long> _repository;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteProductByIdHandler"/> class.
            /// </summary>
            /// <param name="repository">Product repository.</param>
            public DeleteProductByIdHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

            /// <inheritdoc/>
            public async Task<long> Handle(DeleteProductByIdCommand command, CancellationToken ct)
            {
                var product = await _repository.FirstOrDefault(command.Id, ct);

                if (product == null)
                    return default;

                return await _repository.Delete(product.Id, ct);
            }
        }
    }
}