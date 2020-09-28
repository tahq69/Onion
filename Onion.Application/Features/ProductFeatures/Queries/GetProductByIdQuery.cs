using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Queries
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public long Id { get; set; }

        public class GetByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
        {
            private readonly IRepository<Product, long> _repository;

            public GetByIdHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

            public Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken) =>
                _repository.FirstOrDefault(query.Id);
        }
    }
}