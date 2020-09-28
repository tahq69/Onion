using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Onion.Domain.Entities;
using Onion.Identity.Models;

namespace Onion.Identity.Interfaces
{
    /// <summary>
    /// JWT token service contract.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generate JWT Token for provided <param name="user" />.
        /// </summary>
        /// <param name="user">Application user.</param>
        /// <returns>JWT security token.</returns>
        Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user);

        /// <summary>
        /// Serializes a <see cref="JwtSecurityToken"/> into a JWT in Compact Serialization Format.
        /// </summary>
        /// <param name="securityToken"><see cref="JwtSecurityToken"/> to serialize.</param>
        /// <returns>A JWE or JWS in 'Compact Serialization Format'.</returns>
        string WriteToken(JwtSecurityToken securityToken);

        /// <summary>
        /// Generate refresh token.
        /// </summary>
        /// <param name="ipAddress">User IP address.</param>
        /// <returns>Refresh token value.</returns>
        RefreshToken GenerateRefreshToken(string? ipAddress);
    }
}