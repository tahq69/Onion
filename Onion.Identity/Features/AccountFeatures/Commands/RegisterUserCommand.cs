using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Enums;
using Onion.Application.Exceptions;
using Onion.Application.Interfaces;
using Onion.Identity.Models;
using Onion.Infrastructure.Extensions;
using Onion.Infrastructure.Features.EmailFeatures.Commands;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    /// <summary>
    /// User account registration command.
    /// </summary>
    public class RegisterUserCommand : IRequest<Response<string>>
    {
        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets user last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets user email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets account username.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets account password.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// User account registration handler.
        /// </summary>
        public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Response<string>>
        {
            private readonly IEmailService _email;
            private readonly IApiUriService _apiUri;
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="RegisterUserHandler"/> class.
            /// </summary>
            /// <param name="email">Email service.</param>
            /// <param name="apiUri">The API URL service.</param>
            /// <param name="userManager">Application user manager.</param>
            public RegisterUserHandler(
                IEmailService email,
                IApiUriService apiUri,
                UserManager<ApplicationUser> userManager)
            {
                _email = email;
                _apiUri = apiUri;
                _userManager = userManager;
            }

            /// <inheritdoc />
            public async Task<Response<string>> Handle(RegisterUserCommand request, CancellationToken ct)
            {
                var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
                if (userWithSameUserName != null)
                {
                    return Response<string>.Error<string>(
                        nameof(request.UserName),
                        $"Username '{request.UserName}' is already taken.");
                }

                var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithSameEmail != null)
                {
                    return Response<string>.Error<string>(
                        nameof(request.Email),
                        $"Email {request.Email} is already taken.");
                }

                var user = new ApplicationUser
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = new Dictionary<string, ICollection<string>>();
                    foreach (IdentityError? error in result.Errors)
                    {
                        if (errors.ContainsKey(error.Code))
                            errors[error.Code].Add(error.Description);
                        else
                            errors.Add(error.Code, new[] { error.Description });
                    }

                    throw new ValidationException(errors);
                }

                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                await SendVerificationEmail(user);

                return new Response<string>(
                    user.Id,
                    $"User Registered. Please confirm your account by visiting verification URL in your email inbox.");
            }

            private async Task<string> SendVerificationEmail(ApplicationUser user)
            {
                string? code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var uri = _apiUri.GetUri("api/account/confirm-email/", new Dictionary<string, string>
                {
                    { "userId", user.Id },
                    { "code", code },
                }.ToArray()).ToString();

                await _email.Send(
                    user.Email,
                    "Confirm Registration",
                    $"Please confirm your account by visiting this URL {uri}");

                return uri;
            }
        }
    }
}