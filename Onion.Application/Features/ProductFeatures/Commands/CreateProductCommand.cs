﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Application.Features.ProductFeatures.Commands
{
    public class CreateProductCommand : IRequest<long>
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }

        public class CreateProductHandler : IRequestHandler<CreateProductCommand, long>
        {
            private readonly IRepository<Product, long> _repository;

            public CreateProductHandler(IRepository<Product, long> repository)
            {
                _repository = repository;
            }

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