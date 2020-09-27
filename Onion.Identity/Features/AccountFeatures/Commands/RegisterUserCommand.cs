using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Enums;
using Onion.Application.Exceptions;
using Onion.Identity.Models;
using Onion.Infrastructure.Features.EmailFeatures.Commands;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    public class RegisterUserCommand : IRequest<Response<string>>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Origin { get; set; } = null!;

        public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Response<string>>
        {
            private readonly IMediator _mediator;
            private readonly UserManager<ApplicationUser> _userManager;

            public RegisterUserHandler(IMediator mediator, UserManager<ApplicationUser> userManager)
            {
                _mediator = mediator;
                _userManager = userManager;
            }

            public async Task<Response<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
                if (userWithSameUserName != null)
                    throw new AccountException($"Username '{request.UserName}' is already taken.", request.Email);

                var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithSameEmail != null)
                    throw new AccountException($"Email {request.Email} is already taken.", request.Email);

                var user = new ApplicationUser
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    throw new AccountException($"{result.Errors}");

                await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                var verificationUri = await SendVerificationEmail(user, request.Origin);

                return new Response<string>(
                    user.Id,
                    $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
            }
            
            private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
            {
                const string route = "api/account/confirm-email/";

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var endpointUri = new Uri(string.Concat($"{origin}/", route));
                var uri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
                uri = QueryHelpers.AddQueryString(uri, "code", code);

                await _mediator.Send(new SendEmailCommand
                {
                    From = "mail@codewithmukesh.com", // TODO: get from configuration
                    To = user.Email,
                    Body = $"Please confirm your account by visiting this URL {uri}",
                    Subject = "Confirm Registration"
                });

                return uri;
            }
        }
    }
}