using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Onion.Application.DTOs;
using Onion.Application.Interfaces;
using Onion.Identity.Interfaces;

namespace Onion.Identity.Features.TokenFeatures
{
    /// <summary>
    /// Revoke application user token command.
    /// </summary>
    public class RevokeTokenCommand : IRequest<Response<bool>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokeTokenCommand"/> class.
        /// </summary>
        /// <param name="token">Application user refresh token.</param>
        /// <param name="ipAddress">User computer IP address.</param>
        public RevokeTokenCommand(string token, string? ipAddress)
        {
            Token = token;
            IpAddress = ipAddress;
        }

        /// <summary>
        /// Gets or sets application user refresh token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets user computer IP address.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Revoke application user token handler.
        /// </summary>
        public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Response<bool>>
        {
            private readonly IUserRepository _users;
            private readonly IDateTimeService _dateTime;

            /// <summary>
            /// Initializes a new instance of the <see cref="RevokeTokenHandler"/> class.
            /// </summary>
            /// <param name="users">The users repository.</param>
            /// <param name="dateTime">The date time service.</param>
            public RevokeTokenHandler(IUserRepository users, IDateTimeService dateTime)
            {
                _users = users;
                _dateTime = dateTime;
            }

            /// <inheritdoc />
            public async Task<Response<bool>> Handle(RevokeTokenCommand request, CancellationToken ct)
            {
                var user = await _users.SingleByRefreshToken(request.Token, ct);
                if (user is null)
                    return Response<bool>.Error(false, nameof(request.Token), "Token not found");

                var refreshToken = user.RefreshTokens.Single(x => x.Token == request.Token);
                if (!refreshToken.IsActive)
                    return Response<bool>.Error(false, nameof(request.Token), "Token not found");

                refreshToken.Revoked = _dateTime.UtcNow;
                refreshToken.RevokedByIp = request.IpAddress;
                await _users.UpdateRefreshToken(refreshToken, ct);

                return new Response<bool>(true, "Token revoked");
            }
        }
    }
}