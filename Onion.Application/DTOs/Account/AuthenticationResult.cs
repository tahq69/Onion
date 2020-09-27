using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Onion.Application.DTOs.Account
{
    /// <summary>
    /// User account authentication result.
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Gets or sets user identifier.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets account email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets account assigned roles collection.
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether user account email is verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets authentication JWT token.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Gets or sets authentication refresh tokens.
        /// </summary>
        [JsonIgnore]
        public string RefreshToken { get; set; } = null!;
    }
}