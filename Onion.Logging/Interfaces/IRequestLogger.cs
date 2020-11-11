using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// Request logger contract.
    /// </summary>
    public interface IRequestLogger
    {
        /// <summary>
        /// Writes request log if provided <paramref name="level"/> is sufficient.
        /// </summary>
        /// <param name="level">Current logging level.</param>
        /// <param name="context">The HTTP request context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogRequest(ILogger logger, LogLevel level, HttpContext context);
    }
}