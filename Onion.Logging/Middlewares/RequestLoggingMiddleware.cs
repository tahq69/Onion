using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Scopes;
using Onion.Logging.Services;

namespace Onion.Logging.Middlewares
{
    /// <summary>
    /// HTTP request logging middleware.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogContentFactory _contentFactory;
        private readonly LogHeaderFactory _headerFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="contentFactory">The content middleware collection factory.</param>
        /// <param name="headerFactory">The request header middleware collection factory.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="next"/> or <paramref name="contentFactory"/>
        /// or <paramref name="headerFactory"/> or <paramref name="loggerFactory"/> is not provided.
        /// </exception>
        public RequestLoggingMiddleware(
            RequestDelegate next,
            LogContentFactory contentFactory,
            LogHeaderFactory headerFactory,
            ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
            _headerFactory = headerFactory ?? throw new ArgumentNullException(nameof(headerFactory));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _logger = _loggerFactory.CreateLogger<RequestLoggingMiddleware>();
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
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (predicates == null) throw new ArgumentNullException(nameof(predicates));

            // Add controller name to logging hierarchy if available.
            ILogger logger = TryGetControllerLogger(context, _logger);
            string newLine = Environment.NewLine;

            HttpRequest req = context.Request;
            HttpResponse resp = context.Response;

            // Measure request processing time
            IStopwatch stopwatch = CreateStopwatch();
            stopwatch.Start();

            LogLevel level = logger.GetLogLevel();

            string url = $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}";
            string httpRequest = $"{req.Method} {url}{req.QueryString}";

            using var endpointScope = logger.BeginScope(new RequestScope(endpoint: url, req.Method));
            IDisposable? responseScope = null;
            try
            {
                // Apply request predicates to skip request logging.
                bool skip = predicates.Any(i => i.Filter(req));
                if (skip) level = LogLevel.None;

                // Log request details if we use Trace or Debug log level.
                if (level <= LogLevel.Debug)
                {
                    StringBuilder text = new StringBuilder($"{httpRequest} {req.Protocol}{newLine}");
                    WriteHeaders(req.Headers, text);

                    // Write request body content only when we use Trace log level.
                    if (level <= LogLevel.Trace)
                    {
                        req.EnableBuffering();
                        req.Body.Position = 0;

                        string body = await _contentFactory.PrepareBody(req.ContentType, req.Body);
                        text.Append(body);

                        req.Body.Position = 0;

                        logger.LogTrace(text.ToString());
                    }
                    else
                    {
                        // Write url and headers when we use Debug log level.
                        logger.LogDebug(text.ToString());
                    }
                }

                Stream originalBodyStream = context.Response.Body;
                using (var temp = new MemoryStream())
                {
                    // We will read response if using Trace log level.
                    if (level <= LogLevel.Trace) resp.Body = temp;

                    //// ---------------------------------------------------------------------------
                    await _next(context);
                    //// ---------------------------------------------------------------------------

                    stopwatch.Stop();
                    var status = (HttpStatusCode)resp.StatusCode;
                    httpRequest = $"{httpRequest} at {Time(stopwatch)} with {resp.StatusCode} {status}";
                    responseScope = logger.BeginScope(new ResponseScope(resp.StatusCode, stopwatch));

                    // Log response details if we use Trace or Debug log level.
                    if (level <= LogLevel.Debug)
                    {
                        StringBuilder text = new StringBuilder($"{req.Protocol} {resp.StatusCode} {status}{newLine}");

                        WriteHeaders(resp.Headers, text);

                        // Write response body content only when we use Trace log level.
                        if (level <= LogLevel.Trace)
                        {
                            temp.Seek(0, SeekOrigin.Begin);

                            string body = await _contentFactory.PrepareBody(resp.ContentType, temp);
                            text.Append(body);

                            temp.Seek(0, SeekOrigin.Begin);

                            logger.LogTrace(text.ToString());
                            await temp.CopyToAsync(originalBodyStream);
                        }
                        else
                        {
                            // Write status and headers when we use Debug log level.
                            logger.LogDebug(text.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // In case of any error: write Error log message.
                stopwatch.Stop();
                responseScope = logger.BeginScope(new ResponseScope(500, stopwatch));
                httpRequest = $"{httpRequest} at {Time(stopwatch)} with 500 {(HttpStatusCode)500}";

                logger.LogError(e, "Error during http request processing");

                throw;
            }
            finally
            {
                // If using Information, Debug or Trace level, write basic request info.
                if (level <= LogLevel.Information)
                    logger.LogInformation(httpRequest);

                responseScope?.Dispose();
            }
        }

        /// <summary>
        /// Factory method to create Stopwatch wrapper instance.
        /// </summary>
        /// <returns>new SystemStopwatch instance.</returns>
        protected virtual IStopwatch CreateStopwatch()
        {
            return new LoggingStopwatch();
        }

        /// <summary>
        /// Try to create controller specific logger instead or use service specific.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        /// <param name="logger">Actual service logger.</param>
        /// <returns>New controller specific logger or actual on, if could not resolve.</returns>
        private ILogger TryGetControllerLogger(HttpContext context, ILogger logger)
        {
            Endpoint? endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (descriptor != null)
            {
                string controllerName = descriptor.ControllerName;
                string? service = typeof(RequestLoggingMiddleware).FullName;
                logger = _loggerFactory.CreateLogger($"{service}.{controllerName}");
            }

            return logger;
        }

        private void WriteHeaders(IEnumerable<KeyValuePair<string, StringValues>> headers, StringBuilder output)
        {
            foreach (var header in headers)
            {
                string key = header.Key;
                string value = _headerFactory.PrepareHeader(header);

                output.AppendLine($"{key}: {value}");
            }
        }

        private string Time(IStopwatch stopwatch) =>
            stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:fff");
    }
}