using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request/response logger.
    /// </summary>
    public class HttpLogger : IHttpLogger
    {
        private readonly ILogger _logger;
        private readonly IRequestLogger _requestLogger;
        private readonly IResponseLogger _responseLogger;
        private readonly IBasicInfoLogger _basicInfoLogger;
        private readonly IEnumerable<IHttpRequestPredicate> _requestPredicates;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLogger"/> class.
        /// </summary>
        /// <param name="logger">The actual log writer.</param>
        /// <param name="requestLogger">The request logger.</param>
        /// <param name="responseLogger">The response logger.</param>
        /// <param name="basicInfoLogger">The basic information logger.</param>
        /// <param name="requestPredicates">The request predicates to filter unneeded logs.</param>
        public HttpLogger(
            ILogger logger,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            IBasicInfoLogger basicInfoLogger,
            IEnumerable<IHttpRequestPredicate> requestPredicates)
        {
            _logger = logger;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
            _basicInfoLogger = basicInfoLogger;
            _requestPredicates = requestPredicates;
        }

        /// <inheritdoc />
        public async Task LogRequest(RequestDetails request)
        {
            if (ShouldSkip(request))
            {
                return;
            }

            using var requestScope = RequestScope(request);
            await _requestLogger.LogRequest(_logger, request);
        }

        /// <inheritdoc />
        public async Task LogResponse(RequestDetails request, ResponseDetails response)
        {
            if (ShouldSkip(request))
            {
                return;
            }

            using var requestScope = RequestScope(request);
            using var responseScope = ResponseScope(response);
            await _responseLogger.LogResponse(_logger, request, response);
        }

        /// <inheritdoc />
        public Task LogInfo(RequestDetails request, ResponseDetails response)
        {
            if (ShouldSkip(request))
            {
                return Task.CompletedTask;
            }

            using var requestScope = RequestScope(request);
            using var responseScope = ResponseScope(response);

            _basicInfoLogger.LogBasicInfo(_logger, request, response);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void LogError(
            Exception exception,
            RequestDetails? request,
            ResponseDetails? response)
        {
            var statusCode = ((int?)response?.StatusCode) ?? 500;
            ResponseScope scope = new(statusCode, response?.Stopwatch);

            using var requestScope = RequestScope(request);
            using var responseScope = _logger.BeginScope(scope);

            _logger.LogError(exception, "Error during HTTP request processing");
        }

        private IDisposable RequestScope(RequestDetails? request)
        {
            if (request is null) return Disposable.Empty;

            RequestScope scope = new(request.Url, request.Method);

            return _logger.BeginScope(scope);
        }

        private IDisposable ResponseScope(ResponseDetails response)
        {
            ResponseScope scope = new((int)response.StatusCode, response.Stopwatch);

            return _logger.BeginScope(scope);
        }

        private bool ShouldSkip(RequestDetails request)
        {
            return _requestPredicates.Any(predicate => predicate.Filter(request));
        }
    }
}