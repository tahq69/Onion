using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Exceptions;
using Onion.Application.Interfaces;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    /// <summary>
    /// User account email confirmation command.
    /// </summary>
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        /// <summary>
        /// Gets or sets user identifier.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Gets or sets email confirmation code.
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// User account email confirmation handler.
        /// </summary>
        public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Response<string>>
        {
            private readonly IApiUriService _apiUri;
            private readonly IIdentityResultParser _identityParser;
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfirmEmailHandler"/> class.
            /// </summary>
            /// <param name="apiUri">The API URL service.</param>
            /// <param name="userManager">Application user manager.</param>
            /// <param name="identityParser">Identity result parser.</param>
            public ConfirmEmailHandler(
                IApiUriService apiUri,
                IIdentityResultParser identityParser,
                UserManager<ApplicationUser> userManager)
            {
                _apiUri = apiUri;
                _identityParser = identityParser;
                _userManager = userManager;
            }

            /// <inheritdoc/>
            public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user is null)
                    throw ValidationException.FromPropError(nameof(request.UserId), "Invalid user provided.");

                var code = ExtractCode(request);

                IdentityResult? result = await _userManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    return Response<string>.Error<string>(
                        result.Errors?.ElementAt(0)?.Code,
                        result.Errors?.ElementAt(0)?.Description);
                }

                var uri = _apiUri.GetUri("api/Account/authenticate").ToString();
                return new Response<string>(
                    user.Id,
                    $"Account Confirmed for {user.Email}. You can now use the {uri} endpoint.");
            }

            private string ExtractCode(ConfirmEmailCommand request)
            {
                try
                {
                    var bytes = WebEncoders.Base64UrlDecode(request.Code);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch (System.FormatException)
                {
                    throw ValidationException.FromPropError(
                        nameof(request.Code),
                        "Invalid verification code provided.");
                }
            }
        }
    }
}