using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request response logger contract.
    /// </summary>
    public interface IResponseLogger
    {
        /// <summary>
        /// Writes response log if provided <paramref name="level"/> is sufficient.
        /// </summary>
        /// <param name="logger">The actual logger instance.</param>
        /// <param name="level">Current logging level.</param>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogResponse(ILogger logger, LogLevel level, HttpContext context);
    }
}