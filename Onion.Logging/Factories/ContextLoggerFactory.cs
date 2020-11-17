using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;

namespace Onion.Logging.Factories
{
    /// <summary>
    /// HTTP context logger factory.
    /// </summary>
    public class ContextLoggerFactory : IContextLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private readonly IBasicInfoLogger _basicInfoLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextLoggerFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger instance creator.</param>
        /// <param name="requestLogger">The request detail logger.</param>
        /// <param name="responseLogger">The response detail logger.</param>
        /// <param name="basicInfoLogger">The basic request information logger.</param>
        public ContextLoggerFactory(
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            IBasicInfoLogger basicInfoLogger)
        {
            _loggerFactory = loggerFactory;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
            _basicInfoLogger = basicInfoLogger;
        }

        /// <inheritdoc cref="IContextLoggerFactory"/>
        public IContextLogger Create(HttpContext context)
        {
            return new ContextLogger(_loggerFactory, _requestLogger, _responseLogger, _basicInfoLogger, context);
        }
    }
}