using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Application.DTOs;
using Onion.Application.Exceptions;
using Onion.Identity.Models;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; } = null!;
        public string Code { get; set; } = null!;

        public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Response<string>>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public ConfirmEmailHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    return new Response<string>(
                        user.Id,
                        $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
                }

                throw new AccountException($"An error occured while confirming {user.Email} email.", user.Email);
            }
        }
    }
}