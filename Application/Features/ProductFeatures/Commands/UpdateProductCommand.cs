﻿using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.ProductFeatures.Commands
{
    public class UpdateProductCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }

        public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, long>
        {
            private readonly IRepository<Product> _repository;

            public UpdateProductHandler(IRepository<Product> repository)
            {
                _repository = repository;
            }

            public async Task<long> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
            {
                var product = await _repository.FirstOrDefault(command.Id);

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