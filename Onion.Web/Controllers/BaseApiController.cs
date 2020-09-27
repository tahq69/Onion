using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Exceptions;
using Onion.Infrastructure.Extensions;

namespace Onion.Web.Controllers
{
    /// <summary>
    /// Base API controller.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator = null!;

        /// <summary>
        /// Gets encapsulated request/response mediator.
        /// </summary>
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        /// <summary>
        /// Checks controller model state and fails with <see cref="ValidationException"/> if it is not valid.
        /// </summary>
        /// <exception cref="ValidationException">If model state is not valid.</exception>
        protected void AssertModelState()
        {
            if (ModelState.IsValid) return;

            var errors = ModelState.ToDictionary(
                x => x.Key,
                x => x.Value.Errors
                    .Select(y => y.ErrorMessage)
                    .ToCollection());

            throw new ValidationException(errors);
        }
    }
}