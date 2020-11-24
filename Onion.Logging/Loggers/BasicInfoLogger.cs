using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request basic information logger implementation.
    /// </summary>
    public class BasicInfoLogger : IBasicInfoLogger
    {
        /// <inheritdoc cref="IBasicInfoLogger"/>
        public void LogBasicInfo(ILogger logger, LogLevel level, RequestDetails request, ResponseDetails response)
        {
            if (level > LogLevel.Information)
            {
                return;
            }

            var status = $"{(int)response.StatusCode} {response.StatusCode}";
            var message = $"{request.Method} {request.Url} at {response.Time} with {status}";

            logger.LogInformation(message);
        }
    }
}