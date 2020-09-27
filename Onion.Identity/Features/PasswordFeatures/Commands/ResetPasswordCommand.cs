using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Onion.Application.DTOs;
using Onion.Application.Exceptions;
using Onion.Identity.Models;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Password { get; set; } = null!;

        public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Response<string>>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public ResetPasswordHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken ct)
            {
                var account = await _userManager.FindByEmailAsync(request.Email);

                if (account == null)
                    throw new AccountException($"No Accounts Registered with {request.Email}.", request.Email);

                var result = await _userManager.ResetPasswordAsync(account, request.Token, request.Password);
                if (result.Succeeded)
                {
                    return new Response<string>(request.Email, "Password reset completed.");
                }

                var errors = new Dictionary<string, ICollection<string>>();
                foreach (var error in result.Errors)
                {
                    if (errors.ContainsKey(error.Code))
                        errors[error.Code].Add(error.Description);
                    else
                        errors.Add(error.Code, new List<string> { error.Description });
                }

                return new Response<string>("Error occurred while reseting the password.", errors);
            }
        }
    }
}