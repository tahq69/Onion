using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Application.Interfaces;
using Onion.Identity.Features.PasswordFeatures.Commands;
using System.Threading.Tasks;
using Onion.Identity.Features.AccountFeatures.Commands;

namespace Onion.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        [HttpPost("authenticate")]
        public async Task<ActionResult<Response<AuthenticationResponse>>> AuthenticateAsync(
            AuthenticationRequest request)
        {
            var result = await Mediator.Send(new AuthenticateUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GenerateIpAddress(),
            });

            return Ok(result);
        }

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

        private string GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}