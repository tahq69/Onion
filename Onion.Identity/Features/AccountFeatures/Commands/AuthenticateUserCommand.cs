using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Application.Exceptions;
using Onion.Domain.Settings;
using Onion.Identity.Helpers;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    public class AuthenticateUserCommand : IRequest<Response<AuthenticationResult>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string IpAddress { get; set; } = null!;

        public class
            AuthenticateUserHandler : IRequestHandler<AuthenticateUserCommand, Response<AuthenticationResult>>
        {
            private readonly IJwtService _jwt;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;

            public AuthenticateUserHandler(
                IJwtService jwt, 
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager)
            {
                _jwt = jwt;
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task<Response<AuthenticationResult>> Handle(
                AuthenticateUserCommand request,
                CancellationToken ct)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                    throw new AccountException($"No Accounts Registered with {request.Email}.", request.Email);

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    request.Password,
                    false,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                    throw new AccountException($"Invalid Credentials for '{request.Email}'.", request.Email);

                if (!user.EmailConfirmed)
                    throw new AccountException($"Account Not Confirmed for '{request.Email}'.", request.Email);

                JwtSecurityToken securityToken = await _jwt.GenerateJwToken(user);
                string jwToken = _jwt.WriteToken(securityToken);
                IList<string> roles = await _userManager.GetRolesAsync(user);
                RefreshToken refreshToken = _jwt.GenerateRefreshToken(request.IpAddress);

                var response = new AuthenticationResult
                {
                    Id = user.Id,
                    JwToken = jwToken,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles,
                    IsVerified = user.EmailConfirmed,
                    RefreshToken = refreshToken.Token,
                };

                return new Response<AuthenticationResult>(response, $"Authenticated {user.UserName}");
            }
        }
    }
}