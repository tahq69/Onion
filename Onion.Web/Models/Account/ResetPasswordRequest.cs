using System.ComponentModel.DataAnnotations;

namespace Onion.Web.Models.Account
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
    }
}