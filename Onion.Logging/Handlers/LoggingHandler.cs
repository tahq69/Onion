using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Handlers
{
    /// <summary>
    /// HTTP client logging handler.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly IHttpLogger _httpLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingHandler"/> class.
        /// </summary>
        /// <param name="httpLogger">HTTP request logger instance.</param>
        public LoggingHandler(IHttpLogger httpLogger)
        {
            _httpLogger = httpLogger;
        }

        /// <summary>
        /// Send HTTP request asynchronously.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="ct">Request execution cancellation token.</param>
        /// <returns>Executed request response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var stopwatch = CreateStopwatch();
            stopwatch.Start();
            HttpResponseMessage? response = null;
            try
            {
                await _httpLogger.LogRequest(RequestDetails.From(request));

                response = await base.SendAsync(request, ct);

                await _httpLogger.LogResponse(
                    RequestDetails.From(request),
                    ResponseDetails.From(response, stopwatch));
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                _httpLogger.LogError(
                    exception,
                    RequestDetails.From(request),
                    ResponseDetails.From(response, stopwatch));

                throw;
            }
            finally
            {
                _httpLogger.LogInfo(
                    RequestDetails.From(request),
                    ResponseDetails.From(response, stopwatch)).Wait(ct);
            }

            return response;
        }


        /// <summary>
        /// Factory method to create Stopwatch wrapper instance.
        /// </summary>
        /// <returns>New SystemStopwatch instance.</returns>
        protected virtual IStopwatch CreateStopwatch()
        {
            return new LoggingStopwatch();
        }
    }
}