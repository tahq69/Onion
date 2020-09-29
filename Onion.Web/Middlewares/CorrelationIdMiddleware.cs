using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Onion.Domain.Settings;

namespace Onion.Web.Middlewares
{
    /// <summary>
    /// Request correlation identifier middleware. Sets context identifier from
    /// request headers or generates new value for it.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="options">The correlation options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="next"/> or <paramref name="options"/> is not provided.
        /// </exception>
        public CorrelationIdMiddleware(
            RequestDelegate next,
            IOptions<CorrelationIdOptions> options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Invokes middleware the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Next middleware output.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="context"/> is not provided.
        /// </exception>
        public Task Invoke(HttpContext context)
        {
            if (context?.Request?.Headers == null)
                throw new ArgumentNullException(nameof(context));

            // Set trace identifier from header or create new one.
            if (context.Request.Headers.TryGetValue(_options.Header, out StringValues correlationId))
                context.TraceIdentifier = correlationId;
            else
                context.TraceIdentifier = Guid.NewGuid().ToString();

            if (_options.IncludeInResponse)
            {
                // Apply the correlation ID to the response header for client side tracking.
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(_options.Header, new[] { context.TraceIdentifier });
                    return Task.CompletedTask;
                });
            }

            return _next(context);
        }
    }
}