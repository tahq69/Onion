using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    /// <summary>
    /// Create product command.
    /// </summary>
    public class CreateProductCommand : IRequest<long>
    {
        /// <summary>
        /// Gets or sets product name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets product barcode.
        /// </summary>
        public string Barcode { get; set; } = null!;

        /// <summary>
        /// Gets or sets product description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets product rate.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Create product command handler.
        /// </summary>
        public class CreateProductHandler : IRequestHandler<CreateProductCommand, long>
        {
            private readonly IRepository<Product, long> _repository;

            /// <summary>
            /// Initializes a new instance of the <see cref="CreateProductHandler"/> class.
            /// </summary>
            /// <param name="repository">Product repository.</param>
            public CreateProductHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

            /// <inheritdoc/>
            public Task<long> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var product = new Product
                {
                    Barcode = command.Barcode,
                    Name = command.Name,
                    Rate = command.Rate,
                    Description = command.Description,
                };

                return _repository.Insert(product);
            }
        }
    }
}