using Microsoft.Extensions.Logging;

namespace Onion.Logging
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
        /// <param name="request">The HTTP request details.</param>
        /// <param name="response">The HTTP response details.</param>
        void LogBasicInfo(ILogger logger, LogLevel level, RequestDetails request, ResponseDetails response);
    }
}