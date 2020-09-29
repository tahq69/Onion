using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Onion.Application.DTOs;
using Onion.Application.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    /// <summary>
    /// Forgot password command.
    /// </summary>
    public class ForgotPasswordCommand : IRequest<Response<string>>
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Forgot password command handler.
        /// </summary>
        public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Response<string>>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IApiUriService _apiUri;
            private readonly IEmailService _email;

            /// <summary>
            /// Initializes a new instance of the <see cref="ForgotPasswordHandler"/> class.
            /// </summary>
            /// <param name="userManager">Application user manager.</param>
            /// <param name="apiUri">API URI service.</param>
            /// <param name="email">Email service.</param>
            public ForgotPasswordHandler(
                UserManager<ApplicationUser> userManager,
                IApiUriService apiUri,
                IEmailService email)
            {
                _userManager = userManager;
                _apiUri = apiUri;
                _email = email;
            }

            /// <inheritdoc />
            public async Task<Response<string>> Handle(ForgotPasswordCommand request, CancellationToken ct)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var queryParams = new Dictionary<string, string> { { "token", code } };
                var uri = _apiUri.GetUri("api/account/reset-password/", queryParams.ToArray());

                await _email.Send(
                    request.Email,
                    "Password Reset",
                    $"You reset token is - {code}. Please reset password here: {uri}");

                return new Response<string>("E-mail sent.", "E-mail sent.");
            }
        }
    }
}