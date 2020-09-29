using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Onion.Application.DTOs;
using Onion.Application.Exceptions;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    /// <summary>
    /// User account password reset command.
    /// </summary>
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets password reset token.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// User account password reset command handler.
        /// </summary>
        public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Response<string>>
        {
            private readonly IIdentityResultParser _identityParser;
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResetPasswordHandler"/> class.
            /// </summary>
            /// <param name="identityParser">Identity result parser.</param>
            /// <param name="userManager">Application user manager.</param>
            public ResetPasswordHandler(
                IIdentityResultParser identityParser,
                UserManager<ApplicationUser> userManager)
            {
                _identityParser = identityParser;
                _userManager = userManager;
            }

            /// <inheritdoc/>
            public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken ct)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

                if (result.Succeeded)
                    return new Response<string>(request.Email, "Password reset completed.");

                var errors = _identityParser.Errors(result);

                return new Response<string>("Error occurred while resetting the password.", errors);
            }
        }
    }
}