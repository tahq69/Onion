using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Onion.Application.Behaviours
{
    /// <summary>
    /// The model validation behavior.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <seealso cref="MediatR.IPipelineBehavior{TRequest, TResponse}" />
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Handles the specified request and validates model if validators are presented.
        /// </summary>
        /// <param name="request">The request model.</param>
        /// <param name="ct">The asynchronous operation cancellation token.</param>
        /// <param name="next">The next pipeline delegate.</param>
        /// <returns>Next delegate result.</returns>
        /// <exception cref="Exceptions.ValidationException">
        /// When validation of the model fails.
        /// </exception>
        public async Task<TResponse> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var tasks = _validators.Select(v => v.ValidateAsync(context, ct));
            ValidationResult[] validationResults = await Task.WhenAll(tasks);
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
                throw new Exceptions.ValidationException(failures);

            return await next();
        }
    }
}