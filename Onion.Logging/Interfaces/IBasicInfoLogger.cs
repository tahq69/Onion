using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// HTTP request basic information logger contract.
    /// </summary>
    public interface IBasicInfoLogger
    {
        /// <summary>
        /// Logs Request basic information if <paramref name="level"/> is sufficient.
        /// </summary>
        /// <param name="logger">The actual logger to write.</param>
        /// <param name="level">Current request logging level.</param>
        /// <param name="stopwatch">Request execution time measure.</param>
        /// <param name="context">Current request context.</param>
        void LogBasicInfo(ILogger logger, LogLevel level, IStopwatch stopwatch, HttpContext context);
    }
}