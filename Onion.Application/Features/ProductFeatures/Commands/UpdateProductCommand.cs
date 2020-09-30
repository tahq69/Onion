using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    /// <summary>
    /// Update product command.
    /// </summary>
    public class UpdateProductCommand : IRequest<long>
    {
        /// <summary>
        /// Gets or sets product primary key identifier.
        /// </summary>
        public long Id { get; set; }

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
        /// Update product command handler.
        /// </summary>
        public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, long>
        {
            private readonly IRepository<Product, long> _repository;

            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateProductHandler"/> class.
            /// </summary>
            /// <param name="repository">Product repository.</param>
            public UpdateProductHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

            /// <inheritdoc />
            public async Task<long> Handle(UpdateProductCommand command, CancellationToken ct)
            {
                var product = await _repository.FirstOrDefault(command.Id, ct);

                if (product == null)
                    return default;

                product.Barcode = command.Barcode;
                product.Name = command.Name;
                product.Rate = command.Rate;
                product.Description = command.Description;

                return await _repository.Update(product);
            }
        }
    }
}