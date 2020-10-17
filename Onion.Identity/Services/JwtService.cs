using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;
using Onion.Domain.Settings;
using Onion.Identity.Helpers;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Services
{
    /// <summary>
    /// JWT service implementation.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeService _dateTime;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="jwtSettings">JWT service settings.</param>
        /// <param name="dateTime">Date time service.</param>
        /// <param name="userManager">Application user manager.</param>
        public JwtService(
            IOptions<JwtSettings> jwtSettings,
            IDateTimeService dateTime,
            UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _dateTime = dateTime;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public TimeSpan TokenDuration => _jwtSettings.Duration;

        /// <inheritdoc />
        public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            var claims = await GetClaims(user);
            var signingCredentials = SigningCredentials();

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: _dateTime.UtcNow.Add(_jwtSettings.Duration),
                signingCredentials: signingCredentials);
        }

        /// <inheritdoc />
        public string WriteToken(JwtSecurityToken securityToken) =>
            new JwtSecurityTokenHandler().WriteToken(securityToken);

        /// <inheritdoc />
        public RefreshToken GenerateRefreshToken(string? ipAddress) =>
            new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = _dateTime.UtcNow.AddDays(7),
                Created = _dateTime.UtcNow,
                CreatedByIp = ipAddress,
            };

        private SigningCredentials SigningCredentials()
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            return signingCredentials;
        }

        private async Task<IEnumerable<Claim>> GetClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles
                .Select(role => new Claim("roles", role))
                .ToList();

            string ipAddress = IpHelper.GetIpAddress();

            return new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("uid", user.Id),
                    new Claim("ip", ipAddress),
                }
                .Union(userClaims)
                .Union(roleClaims);
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", string.Empty);
        }
    }
}