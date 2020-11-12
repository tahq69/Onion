using Microsoft.AspNetCore.Http;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// HTTP context logger factory contract.
    /// </summary>
    public interface IContextLoggerFactory
    {
        /// <summary>
        /// Create HTTP context logger instance for provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        /// <returns>New instance of the <seealso cref="IContextLogger"/>.</returns>
        IContextLogger Create(HttpContext context);
    }
}