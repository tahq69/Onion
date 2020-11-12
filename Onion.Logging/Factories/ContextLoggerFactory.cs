using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;

namespace Onion.Logging.Factories
{
    public class ContextLoggerFactory : IContextLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private readonly IBasicInfoLogger _basicInfoLogger;

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

        public IContextLogger Create(HttpContext context)
        {
            return new ContextLogger(_loggerFactory, _requestLogger, _responseLogger, _basicInfoLogger, context);
        }
    }
}