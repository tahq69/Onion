using System.ComponentModel.DataAnnotations;

namespace Onion.Web.Models.Account
{
    /// <summary>
    /// User account password reset request model.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Gets or sets user account email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets password reset token.
        /// </summary>
        [Required]
        public string? Token { get; set; }

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets new password confirmation.
        /// </summary>
        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}