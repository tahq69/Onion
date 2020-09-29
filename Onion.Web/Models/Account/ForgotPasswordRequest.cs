using System.ComponentModel.DataAnnotations;

namespace Onion.Web.Models.Account
{
    /// <summary>
    /// Account forgot password request model.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}