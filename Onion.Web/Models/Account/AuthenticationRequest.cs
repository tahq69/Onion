using System.ComponentModel.DataAnnotations;

namespace Onion.Web.Models.Account
{
    /// <summary>
    /// User authentication request.
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets user account password.
        /// </summary>
        [Required]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets user indicator to be remembered on this PC.
        /// </summary>
        public bool? Remember { get; set; }
    }
}