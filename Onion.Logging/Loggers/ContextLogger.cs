using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP context logger.
    /// </summary>
    /// <typeparam name="T">Type of the logger instance name.</typeparam>
    public class ContextLogger<T> : IContextLogger
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IHttpRequestPredicate> _predicates;
        private readonly HttpContext _context;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private readonly IBasicInfoLogger _basicInfoLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextLogger{T}"/> class.
        /// </summary>
        /// <param name="predicates">Collection of predicates to be used to exclude request from logging.</param>
        /// <param name="loggerFactory">Logger instance creator.</param>
        /// <param name="requestLogger">The request detail logger.</param>
        /// <param name="responseLogger">The response detail logger.</param>
        /// <param name="basicInfoLogger">The basic request information logger.</param>
        /// <param name="context">The request context.</param>
        public ContextLogger(
            IEnumerable<IHttpRequestPredicate> predicates,
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            IBasicInfoLogger basicInfoLogger,
            HttpContext context)
        {
            _logger = loggerFactory.ControllerLogger<T>(context);
            _context = context;
            _predicates = predicates;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
            _basicInfoLogger = basicInfoLogger;
        }

        /// <inheritdoc cref="IContextLogger" />
        public LogLevel LogLevel => GetLogLevel();

        /// <inheritdoc cref="IContextLogger" />
        public async Task LogRequest()
        {
            using var requestScope = RequestScope();

            await _requestLogger.LogRequest(_logger, LogLevel, _context.Request);
        }

        /// <inheritdoc cref="IContextLogger" />
        public async Task LogResponse(IStopwatch stopwatch)
        {
            using var requestScope = RequestScope();
            using var responseScope = ResponseScope(stopwatch);

            await _responseLogger.LogResponse(_logger, LogLevel, _context.Request, _context.Response);
        }

        /// <inheritdoc cref="IContextLogger" />
        public void LogInfo(IStopwatch stopwatch)
        {
            var request = RequestDetails.From(_context.Request);
            var response = ResponseDetails.From(_context.Response, stopwatch);

            using var requestScope = RequestScope();
            using var responseScope = ResponseScope(stopwatch);

            _basicInfoLogger.LogBasicInfo(_logger, LogLevel, request, response);
        }

        /// <inheritdoc cref="IContextLogger" />
        public void LogError(Exception exception, IStopwatch? stopwatch)
        {
            var statusCode = _context.Response?.StatusCode ?? 500;
            ResponseScope scope = new(statusCode, stopwatch);

            using var requestScope = RequestScope();
            using var responseScope = _logger.BeginScope(scope);

            _logger.LogError(exception, "Error during http request processing");
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

        private LogLevel GetLogLevel()
        {
            if (ShouldSkip())
            {
                return LogLevel.None;
            }

            return _logger.GetLogLevel();
        }

        private bool ShouldSkip()
        {
            return _predicates.Any(predicate => predicate.Filter(_context.Request));
        }
    }
}