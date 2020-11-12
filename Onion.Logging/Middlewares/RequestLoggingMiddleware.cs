using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;
using Onion.Logging.Services;

namespace Onion.Logging.Middlewares
{
    /// <summary>
    /// HTTP request logging middleware.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IContextLoggerFactory _contextLoggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="contextLoggerFactory">HTTP context logger factory service.</param>
        public RequestLoggingMiddleware(
            RequestDelegate next,
            IContextLoggerFactory contextLoggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _contextLoggerFactory = contextLoggerFactory;
        }

        /// <summary>
        /// <list type="bullet">
        ///   <item>
        ///     <term>Information</term>
        ///     <description>Single entry per request/response with status and execution time.</description>
        ///   </item>
        ///   <item>
        ///     <term>Debug</term>
        ///     <description>Request/response logged separate entries with headers.</description>
        ///   </item>
        ///   <item>
        ///     <term>Verbose</term>
        ///     <description>Request/response logged separate entries with headers and body.</description>
        ///   </item>
        /// </list>
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="predicates">Collection of predicates to be used to exclude request from logging.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context, IEnumerable<IHttpRequestPredicate> predicates)
        {
            var logger = _contextLoggerFactory.Create(context);
            var level = Level(context, predicates, logger);
            var stopwatch = CreateStopwatch();
            stopwatch.Start();

            try
            {
                await logger.LogRequest(level);

                Stream originalBodyStream = context.Response.Body;
                await using MemoryStream temp = new();
                if (level <= LogLevel.Trace)
                {
                    context.Response.Body = temp;
                }

                // -------------------------------------------------------------
                await _next(context);
                // -------------------------------------------------------------

                stopwatch.Stop();

                await logger.LogResponse(level, stopwatch);
                if (level <= LogLevel.Trace)
                {
                    await temp.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                logger.LogError(exception, stopwatch);

                throw;
            }
            finally
            {
                logger.LogInfo(level, stopwatch);
            }
        }

        /// <summary>
        /// Factory method to create Stopwatch wrapper instance.
        /// </summary>
        /// <returns>New SystemStopwatch instance.</returns>
        protected virtual IStopwatch CreateStopwatch()
        {
            return new LoggingStopwatch();
        }

        private static LogLevel Level(
            HttpContext context,
            IEnumerable<IHttpRequestPredicate> predicates,
            IContextLogger logger)
        {
            if (ShouldSkip(context, predicates))
            {
                return LogLevel.None;
            }

            return logger.GetLogLevel();
        }

        private static bool ShouldSkip(HttpContext context, IEnumerable<IHttpRequestPredicate> predicates)
        {
            return predicates.Any(predicate => predicate.Filter(context.Request));
        }
    }
}