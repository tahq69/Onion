using System.ComponentModel.DataAnnotations;

namespace Onion.Web.Models.Account
{
    /// <summary>
    /// Account registration request model.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Required]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets user last name.
        /// </summary>
        [Required]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets user email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets account username.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets account password.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets account password confirmation.
        /// </summary>
        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}