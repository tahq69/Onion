using System.Threading.Tasks;

namespace Onion.Application.Interfaces
{
    /// <summary>
    /// Email service contract.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="to">Email recipient.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body message.</param>
        /// <param name="from">Email sender address (Configuration value used if is not provided).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Send(string to, string subject, string body, string? from = null);
    }
}