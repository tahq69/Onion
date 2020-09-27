using MediatR;
using Microsoft.AspNetCore.Identity;
using Onion.Identity.Models;
using Onion.Infrastructure.Features.EmailFeatures.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using Onion.Application.DTOs;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    public class ForgotPasswordCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = null!;

        public string Origin { get; set; } = null!;

        public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Response<string>>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IMediator _mediator;

            public ForgotPasswordHandler(UserManager<ApplicationUser> userManager, IMediator mediator)
            {
                _userManager = userManager;
                _mediator = mediator;
            }

            public async Task<Response<string>> Handle(ForgotPasswordCommand request, CancellationToken ct)
            {
                ApplicationUser account = await _userManager.FindByEmailAsync(request.Email);

                if (account == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Invalid e-mail address",
                    };
                }

                string code = await _userManager.GeneratePasswordResetTokenAsync(account);
                var route = "api/account/reset-password/";
                var endpointUri = new Uri(string.Concat($"{request.Origin}/", route));

                await _mediator.Send(new SendEmailCommand
                {
                    Body = $"You reset token is - {code}. Please reset password here: {endpointUri}",
                    To = request.Email,
                    Subject = $"{request.Origin} Password Reset",
                });

                return new Response<string>(null, "E-mail sent.");
            }
        }
    }
}