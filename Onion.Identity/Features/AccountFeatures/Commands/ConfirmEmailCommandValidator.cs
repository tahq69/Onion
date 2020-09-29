using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.WebUtilities;
using Onion.Identity.Interfaces;

namespace Onion.Identity.Features.AccountFeatures.Commands
{
    /// <summary>
    /// Email confirmation command validator.
    /// </summary>
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        private readonly IUserRepository _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailCommandValidator"/> class.
        /// </summary>
        /// <param name="users">Application user manager.</param>
        public ConfirmEmailCommandValidator(IUserRepository users)
        {
            _users = users;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .Must(BeBase64String).WithMessage("{PropertyName} must be valid code.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MustAsync(ExistInDb).WithMessage("{PropertyName} must be valid identifier.");
        }

        private async Task<bool> ExistInDb(string userId, CancellationToken ct) =>
            await _users.ExistsById(userId, ct);

        private bool BeBase64String(string code)
        {
            try
            {
                var bytes = WebEncoders.Base64UrlDecode(code);
                Encoding.UTF8.GetString(bytes);

                return true;
            }
            catch (System.FormatException)
            {
                return false;
            }
        }
    }
}