using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ProductFeatures.Commands
{
    public class DeleteProductByIdCommand : IRequest<long>
    {
        public long Id { get; set; }

        public class DeleteProductByIdHandler : IRequestHandler<DeleteProductByIdCommand, long>
        {
            private readonly IRepository<Product> _repository;

            public DeleteProductByIdHandler(IRepository<Product> repository)
            {
                _repository = repository;
            }

            public async Task<long> Handle(DeleteProductByIdCommand command, CancellationToken cancellationToken)
            {
                var product = await _repository.FirstOrDefault(command.Id);

                if (product == null)
                    return default;

                return await _repository.Delete(product.Id);
            }
        }
    }
}