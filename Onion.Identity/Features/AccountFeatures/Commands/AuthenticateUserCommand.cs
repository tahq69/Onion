using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Domain.Entities;
using Onion.Identity.Contexts;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    /// <summary>
    /// Execute user authentication command details.
    /// </summary>
    public class AuthenticateUserCommand : IRequest<Response<AuthenticationResult>>
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets user account password.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets user IP address.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User authentication command handler.
        /// </summary>
        public class AuthenticateUserHandler :
            IRequestHandler<AuthenticateUserCommand, Response<AuthenticationResult>>
        {
            private readonly ILogger<AuthenticateUserHandler> _logger;
            private readonly IJwtService _jwt;
            private readonly IUserRepository _users;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="AuthenticateUserHandler"/> class.
            /// </summary>
            /// <param name="logger">Service specific logger instance.</param>
            /// <param name="jwt">JWT authentication service.</param>
            /// <param name="users">Users data repository.</param>
            /// <param name="userManager">User record manager.</param>
            /// <param name="signInManager">User sign in manager.</param>
            public AuthenticateUserHandler(
                ILogger<AuthenticateUserHandler> logger,
                IJwtService jwt,
                IUserRepository users,
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager)
            {
                _logger = logger;
                _jwt = jwt;
                _users = users;
                _userManager = userManager;
                _signInManager = signInManager;
            }

            /// <inheritdoc />
            public async Task<Response<AuthenticationResult>> Handle(
                AuthenticateUserCommand request,
                CancellationToken ct)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    _logger.LogWarning("No Accounts Registered with {Email} address.", request.Email);
                    return InvalidCredentials();
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    request.Password,
                    false,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Invalid Credentials for '{Email}'.", request.Email);
                    return InvalidCredentials();
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Account Not Confirmed for '{Email}'.", request.Email);
                    return InvalidCredentials($"Account email address '{request.Email}' is not confirmed.");
                }

                JwtSecurityToken jwtToken = await _jwt.GenerateJwtToken(user);
                string token = _jwt.WriteToken(jwtToken);
                IList<string> roles = await _userManager.GetRolesAsync(user);

                RefreshToken refreshToken = _jwt.GenerateRefreshToken(request.IpAddress);
                await _users.AddRefreshToken(user.Id, refreshToken, ct);

                var response = new AuthenticationResult
                {
                    Id = user.Id,
                    Token = token,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles,
                    IsVerified = user.EmailConfirmed,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = _jwt.TokenDuration.TotalSeconds,
                };

                return new Response<AuthenticationResult>(response, $"Authenticated {user.UserName}");
            }

            private Response<AuthenticationResult> InvalidCredentials(
                string message = "Invalid credentials provided.") =>
                new Response<AuthenticationResult>
                {
                    Succeeded = false,
                    Message = message,
                    Errors = new Dictionary<string, ICollection<string>>
                    {
                        { nameof(AuthenticationResult.Email), new[] { message } },
                    },
                };
        }
    }
}