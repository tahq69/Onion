using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Application.Interfaces;
using Onion.Identity.Contexts;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Features.TokenFeatures
{
    /// <summary>
    /// Refresh user authentication token command.
    /// </summary>
    public class RefreshTokenCommand : IRequest<Response<AuthenticationResult>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenCommand"/> class.
        /// </summary>
        /// <param name="ipAddress">User computer IP address.</param>
        /// <param name="refreshToken">User Account refresh token.</param>
        public RefreshTokenCommand(string? ipAddress, string refreshToken)
        {
            IpAddress = ipAddress;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Gets or sets user computer IP address.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets user Account refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Refresh token command handler.
        /// </summary>
        public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Response<AuthenticationResult>>
        {
            private readonly IdentityDbContext _dbContext;
            private readonly DbSet<ApplicationUser> _entities;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IJwtService _jwt;
            private readonly IDateTimeService _dateTime;

            /// <summary>
            /// Initializes a new instance of the <see cref="RefreshTokenHandler"/> class.
            /// </summary>
            /// <param name="dbContext">Identity database context.</param>
            /// <param name="userManager">Identity user manager.</param>
            /// <param name="jwt">JWT token service.</param>
            /// <param name="dateTime">Date time object service.</param>
            public RefreshTokenHandler(
                IdentityDbContext dbContext,
                UserManager<ApplicationUser> userManager,
                IJwtService jwt,
                IDateTimeService dateTime)
            {
                _dbContext = dbContext;
                _userManager = userManager;
                _entities = dbContext.Set<ApplicationUser>();
                _jwt = jwt;
                _dateTime = dateTime;
            }

            /// <inheritdoc />
            public async Task<Response<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken ct)
            {
                var user = await _entities.SingleOrDefaultAsync(
                    u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), ct);

                if (user == null)
                    return CreateFailureResponse("Invalid refresh token value.");

                var refreshToken = user.RefreshTokens.Single(x => x.Token == request.RefreshToken);
                if (!refreshToken.IsActive)
                    return CreateFailureResponse("Refresh token is already expired.");

                RefreshToken newRefreshToken = await UpdateRefreshToken(refreshToken, user, request.IpAddress, ct);

                var jwtToken = await _jwt.GenerateJwtToken(user);
                var token = _jwt.WriteToken(jwtToken);
                IList<string> roles = await _userManager.GetRolesAsync(user);
                var response = new AuthenticationResult()
                {
                    Id = user.Id,
                    Token = token,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles,
                    IsVerified = user.EmailConfirmed,
                    RefreshToken = newRefreshToken.Token,
                };

                return new Response<AuthenticationResult>(response, "User token updated.");
            }

            private async Task<RefreshToken> UpdateRefreshToken(
                RefreshToken refreshToken,
                ApplicationUser user,
                string? ipAddress,
                CancellationToken ct)
            {
                RefreshToken newRefreshToken = _jwt.GenerateRefreshToken(ipAddress);
                refreshToken.Revoked = _dateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                refreshToken.ReplacedByToken = newRefreshToken.Token;

                user.RefreshTokens.Add(newRefreshToken);
                _dbContext.Update(user);

                await _dbContext.SaveChangesAsync(ct);

                return newRefreshToken;
            }

            private Response<AuthenticationResult> CreateFailureResponse(string message) =>
                new Response<AuthenticationResult>
                {
                    Succeeded = false,
                    Message = message,
                };
        }
    }
}