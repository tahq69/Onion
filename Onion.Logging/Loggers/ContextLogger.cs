using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;
using Onion.Logging.Scopes;

namespace Onion.Logging.Loggers
{
    public class ContextLogger : IContextLogger
    {
        private readonly ILogger _logger;
        private readonly HttpContext _context;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private readonly IBasicInfoLogger _basicInfoLogger;

        public ContextLogger(
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            IBasicInfoLogger basicInfoLogger,
            HttpContext context)
        {
            _logger = loggerFactory.ControllerLogger(context);
            _context = context;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
            _basicInfoLogger = basicInfoLogger;
        }

        public async Task LogRequest(LogLevel level)
        {
            using var requestScope = RequestScope();

            await _requestLogger.LogRequest(_logger, level, _context);
        }

        public async Task LogResponse(LogLevel level, IStopwatch stopwatch)
        {
            using var requestScope = RequestScope();
            using var responseScope = ResponseScope(stopwatch);

            await _responseLogger.LogResponse(_logger, level, stopwatch, _context);
        }

        public void LogInfo(LogLevel level, IStopwatch stopwatch)
        {
            using var requestScope = RequestScope();
            using var responseScope = ResponseScope(stopwatch);

            _basicInfoLogger.LogBasicInfo(_logger, level, stopwatch, _context);
        }

        public void LogError(Exception exception, IStopwatch stopwatch)
        {
            var statusCode = _context.Response?.StatusCode ?? 500;
            ResponseScope scope = new(statusCode, stopwatch);

            using var requestScope = RequestScope();
            using var responseScope = _logger.BeginScope(scope);

            _logger.LogError(exception, "Error during http request processing");
        }

        public LogLevel GetLogLevel()
        {
            return _logger.GetLogLevel();
        }

        private IDisposable RequestScope()
        {
            string url = _context.Request.GetDisplayUrl();
            RequestScope scope = new(url, _context.Request.Method);

            return _logger.BeginScope(scope);
        }

        private IDisposable ResponseScope(IStopwatch stopwatch)
        {
            ResponseScope scope = new(_context.Response.StatusCode, stopwatch);

            return _logger.BeginScope(scope);
        }
    }
}