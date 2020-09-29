using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Web.Middlewares
{
    /// <summary>
    /// Request correlation identifier logging middleware.
    /// </summary>
    public class CorrelationIdLoggingMiddleware
    {
        private readonly ILogger<CorrelationIdLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="next">The next request delegate.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="logger"/> or <paramref name="next"/> is not provided.
        /// </exception>
        public CorrelationIdLoggingMiddleware(ILogger<CorrelationIdLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invokes action the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Next middleware output.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="context"/> is not provided.
        /// </exception>
        public Task Invoke(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scope = new Dictionary<string, object>
            {
                { "CorrelationId", context.TraceIdentifier },
            };

            using (_logger.BeginScope(scope))
            {
                return _next(context);
            }
        }
    }
}