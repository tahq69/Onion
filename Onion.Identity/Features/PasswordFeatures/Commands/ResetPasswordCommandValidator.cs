using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Onion.Identity.Interfaces;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    /// <summary>
    /// User account password reset command validator.
    /// </summary>
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly IUserRepository _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordCommandValidator"/> class.
        /// </summary>
        /// <param name="users">Application users repository.</param>
        public ResetPasswordCommandValidator(IUserRepository users)
        {
            _users = users;

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .EmailAddress().WithMessage("{PropertyName} must be valid email.")
                .MustAsync(ExistInDb).WithMessage("{PropertyName} must exist.");

            // TODO: add password validation.
        }

        private Task<bool> ExistInDb(string userEmail, CancellationToken ct) =>
            _users.ExistsByEmail(userEmail, ct);
    }
}