using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.DTOs;
using Onion.Application.Features.ProductFeatures.Commands;
using Onion.Application.Features.ProductFeatures.Queries;
using Onion.Domain.Entities;

namespace Onion.Web.Controllers.v1
{
    /// <summary>
    /// Product API 1.0 HTTP request controller.
    /// </summary>
    /// <seealso cref="Onion.Web.Controllers.BaseApiController" />
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductController : BaseApiController
    {
        /// <summary>
        /// Gets paginated product records.
        /// </summary>
        /// <param name="filter">The pagination filters.</param>
        /// <returns>Paginated product records.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<ICollection<Product>>>> GetAll([FromQuery] PaginationFilter filter)
        {
            var validFilter = filter.ValidValues();
            var results = await Mediator.Send(new GetProductsCommand
            {
                PageNumber = validFilter.PageNumber,
                PageSize = validFilter.PageSize,
                Route = Request.Path.Value,
            });

            return Ok(results);
        }


        /// <summary>
        /// Creates a New Product.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<long>> Create(CreateProductCommand command)
        {
            var id = await Mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new {id}, id);
        }

        /// <summary>
        /// Gets Product Entity by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var query = new GetProductByIdQuery {Id = id};
            var product = await Mediator.Send(query);

            if (product == null)
                return NoContent();

            return Ok(product);
        }

        /// <summary>
        /// Deletes Product Entity based on Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<long>> Delete(int id)
        {
            var command = new DeleteProductByIdCommand {Id = id};
            var result = await Mediator.Send(command);

            if (result == default)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Updates the Product Entity based on Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("[action]")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> Update(int id, UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var result = await Mediator.Send(command);
            if (result == default)
                return NoContent();

            return AcceptedAtAction(nameof(GetById), new {id}, result);
        }
    }
}