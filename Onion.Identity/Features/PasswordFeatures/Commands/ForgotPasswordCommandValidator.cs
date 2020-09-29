using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Onion.Identity.Interfaces;

namespace Onion.Identity.Features.PasswordFeatures.Commands
{
    /// <summary>
    /// Forgot password command validator.
    /// </summary>
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        private readonly IUserRepository _users;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordCommandValidator"/> class.
        /// </summary>
        /// <param name="users">Application user record repository.</param>
        public ForgotPasswordCommandValidator(IUserRepository users)
        {
            _users = users;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .EmailAddress().WithMessage("{PropertyName} must be valid email.")
                .MustAsync(ExistInDb).WithMessage("{PropertyName} must exist.");
        }

        private Task<bool> ExistInDb(string userEmail, CancellationToken ct) =>
            _users.ExistsByEmail(userEmail, ct);
    }
}