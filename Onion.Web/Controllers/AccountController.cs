using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Identity.Features.AccountFeatures.Commands;
using Onion.Identity.Features.PasswordFeatures.Commands;
using Onion.Web.Models.Account;

namespace Onion.Web.Controllers
{
    /// <summary>
    /// User account controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        /// <summary>
        /// Authenticate user.
        /// </summary>
        /// <param name="request">The request model.</param>
        /// <returns>Authentication result.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<AuthenticationResult>>> AuthenticateAsync(
            AuthenticationRequest request)
        {
            AssertModelState();

            var result = await Mediator.Send(new AuthenticateUserCommand
            {
                Email = request.Email ?? throw new ArgumentNullException(nameof(request.Email)),
                Password = request.Password ?? throw new ArgumentNullException(nameof(request.Password)),
                IpAddress = GenerateIpAddress(),
            });

            if (!result.Succeeded)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Create new user account.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<Response<string>>> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            var result = await Mediator.Send(new RegisterUserCommand
            {
                Origin = origin,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
            });

            return Ok(result);
        }

        /// <summary>
        /// Confirm user email.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var result = await Mediator.Send(new ConfirmEmailCommand
            {
                Code = code,
                UserId = userId,
            });

            return Ok(result);
        }

        /// <summary>
        /// Send password renew link to user email.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var result = await Mediator.Send(new ForgotPasswordCommand
            {
                Email = model.Email,
                Origin = Request.Headers["origin"],
            });

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Response<string>>> ResetPassword(ResetPasswordRequest model)
        {
            var result = await Mediator.Send(new ResetPasswordCommand
            {
                Email = model.Email,
                Password = model.Password,
                Token = model.Token,
            });

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        private string? GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
        }
    }
}