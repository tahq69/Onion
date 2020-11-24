using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
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
        private readonly IEnumerable<IHttpRequestPredicate> _predicates;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextLoggerFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">Logger instance creator.</param>
        /// <param name="requestLogger">The request detail logger.</param>
        /// <param name="responseLogger">The response detail logger.</param>
        /// <param name="basicInfoLogger">The basic request information logger.</param>
        /// <param name="predicates">Collection of predicates to be used to exclude request from logging.</param>
        public ContextLoggerFactory(
            ILoggerFactory loggerFactory,
            IRequestLogger requestLogger,
            IResponseLogger responseLogger,
            IBasicInfoLogger basicInfoLogger,
            IEnumerable<IHttpRequestPredicate> predicates)
        {
            _loggerFactory = loggerFactory;
            _requestLogger = requestLogger;
            _responseLogger = responseLogger;
            _basicInfoLogger = basicInfoLogger;
            _predicates = predicates;
        }

        /// <inheritdoc cref="IContextLoggerFactory"/>
        public IContextLogger Create<T>(HttpContext context)
        {
            return new ContextLogger<T>(
                _predicates,
                _loggerFactory,
                _requestLogger,
                _responseLogger,
                _basicInfoLogger,
                context);
        }
    }
}