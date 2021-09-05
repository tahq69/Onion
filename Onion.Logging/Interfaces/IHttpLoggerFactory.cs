using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP logger factory contract.
    /// </summary>
    public interface IHttpLoggerFactory
    {
        /// <summary>
        /// Create HTTP logger.
        /// </summary>
        /// <param name="logger">Application logger to use as HTTP logger writer.</param>
        /// <returns>HTTP logger instance.</returns>
        IHttpLogger Create(ILogger logger);
    }
}