using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;
using Onion.Logging.Scopes;

namespace Onion.Logging.Loggers
{
    public class ContextLogger : IContextLogger
    {
        private readonly HttpContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private ILogger _logger;

        public ContextLogger(
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger)
        {
            _loggerFactory = loggerFactory;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
        }

        private ContextLogger(
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            HttpContext context)
            : this(loggerFactory, requestLogger, responseLogger)
        {
            _context = context;
            _logger = GetLogger(context);
        }

        public IContextLogger FromContext(HttpContext context)
        {
            return new ContextLogger(_loggerFactory, _requestLogger, _responseLogger, context);
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

        public void LogInfo(IStopwatch stopwatch)
        {
            string method = _context.Request.Method;
            string url = _context.Request.GetDisplayUrl();
            var code = _context.Response.StatusCode;
            var status = $"{code} {(HttpStatusCode)code}";

            using var requestScope = RequestScope();
            using var responseScope = ResponseScope(stopwatch);

            _logger.LogInformation($"{method} {url} at {Time(stopwatch)} with {status}");
        }

        public void LogError(Exception exception, IStopwatch stopwatch)
        {
            var statusCode = _context.Response?.StatusCode ?? 500;
            var scope = new ResponseScope(statusCode, stopwatch);

            using var requestScope = RequestScope();
            using var responseScope = _logger.BeginScope(scope);

            _logger.LogError(exception, "Error during http request processing");
        }

        public LogLevel GetLogLevel()
        {
            return _logger.GetLogLevel();
        }

        private ILogger GetLogger(HttpContext context)
        {
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (descriptor != null)
            {
                string? service = typeof(RequestLoggingMiddleware).FullName;
                string controllerName = descriptor.ControllerName;
                return _loggerFactory.CreateLogger($"{service}.{controllerName}");
            }

            return _loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        private IDisposable RequestScope()
        {
            string url = _context.Request.GetDisplayUrl();
            var scope = new RequestScope(url, _context.Request.Method);

            return _logger.BeginScope(scope);
        }

        private IDisposable ResponseScope(IStopwatch stopwatch)
        {
            var scope = new ResponseScope(_context.Response.StatusCode, stopwatch);

            return _logger.BeginScope(scope);
        }

        private string Time(IStopwatch stopwatch) =>
            stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:fff");
    }
}