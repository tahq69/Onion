// ReSharper disable NotResolvedInText

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Identity.Features.AccountFeatures.Commands;
using Onion.Identity.Features.PasswordFeatures.Commands;
using Onion.Identity.Features.TokenFeatures;
using Onion.Web.Models.Account;
using StringResponse = Microsoft.AspNetCore.Mvc.ActionResult<Onion.Application.DTOs.Response<string>>;

namespace Onion.Web.Controllers
{
    /// <summary>
    /// User account controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private const string TokenCookieKey = "refreshToken";

        private bool HasTokenCookie => Request.Cookies?.ContainsKey(TokenCookieKey) ?? false;

        private string? TokenCookie => Request.Cookies[TokenCookieKey];

        /// <summary>
        /// Authenticate user.
        /// </summary>
        /// <param name="request">The request model.</param>
        /// <returns>Authentication result.</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<AuthenticationResult>>> AuthenticateAsync(AuthenticationRequest request)
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

            SetTokenCookie(result.Data.RefreshToken);

            return Ok(result);
        }

        /// <summary>
        /// Refresh token value from cookie.
        /// </summary>
        /// <returns>User details with new token value.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<AuthenticationResult>>> RefreshTokenAsync()
        {
            if (!HasTokenCookie)
            {
                return BadRequest(new Response<AuthenticationResult>(
                    "Cookies does not contain required refresh token."));
            }

            string? ipAddress = GenerateIpAddress();

            string refreshToken = TokenCookie ?? throw new ArgumentNullException("Token");
            var cmd = new RefreshTokenCommand(ipAddress, refreshToken);
            var result = await Mediator.Send(cmd);

            if (!result.Succeeded)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Refresh token value from cookie.
        /// </summary>
        /// <param name="request">The revoke token request.</param>
        /// <returns>User details with new token value.</returns>
        [Authorize]
        [HttpPost("revoke-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> RevokeTokenAsync(RevokeTokenRequest request)
        {
            var token = request?.Token;
            if (string.IsNullOrWhiteSpace(token) && HasTokenCookie)
                token = TokenCookie;

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(Response<bool>.Error(
                    false,
                    nameof(request.Token),
                    "Token is required"));
            }

            var ipAddress = GenerateIpAddress();
            Response<bool> result = await Mediator.Send(new RevokeTokenCommand(token, ipAddress));

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Create new user account.
        /// </summary>
        /// <param name="request">Account registration request.</param>
        /// <returns>Email confirmation details.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<StringResponse> RegisterAsync(RegisterRequest request)
        {
            AssertModelState();

            var result = await Mediator.Send(new RegisterUserCommand
            {
                Email = request.Email ?? throw new ArgumentNullException(nameof(request.Email)),
                Password = request.Password ?? throw new ArgumentNullException(nameof(request.Password)),
                FirstName = request.FirstName ?? throw new ArgumentNullException(nameof(request.FirstName)),
                LastName = request.LastName ?? throw new ArgumentNullException(nameof(request.LastName)),
                UserName = request.UserName ?? throw new ArgumentNullException(nameof(request.UserName)),
            });

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Confirm user email.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="code">Email confirmation code.</param>
        /// <returns>Login URL if email is confirmed.</returns>
        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<StringResponse> ConfirmEmailAsync(
            [FromQuery] string userId,
            [FromQuery] string code)
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
        /// <param name="request">Forgot password request model.</param>
        /// <returns>Forgot password email sent details.</returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<StringResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var result = await Mediator.Send(new ForgotPasswordCommand
            {
                Email = request.Email ?? throw new ArgumentNullException(nameof(request.Email)),
            });

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
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
            AssertModelState();

            var result = await Mediator.Send(new ResetPasswordCommand
            {
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email)),
                Password = model.Password ?? throw new ArgumentNullException(nameof(model.Password)),
                Token = model.Token ?? throw new ArgumentNullException(nameof(model.Token)),
            });

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        #region Helper methods

        private string? GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
        }


        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            Response.Cookies.Append(TokenCookieKey, token, cookieOptions);
        }

        #endregion
    }
}