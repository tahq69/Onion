using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request response logger.
    /// </summary>
    public class ResponseLogger : ContentLogger, IResponseLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseLogger"/> class.
        /// </summary>
        /// <param name="contentFactory">Request content value factory.</param>
        /// <param name="headerFactory">Request header value factory.</param>
        public ResponseLogger(
            LogContentFactory contentFactory,
            LogHeaderFactory headerFactory)
            : base(headerFactory, contentFactory)
        {
        }

        /// <inheritdoc cref="IResponseLogger"/>
        public async Task LogResponse(ILogger logger, LogLevel level, HttpRequest request, HttpResponse response)
        {
            if (level > LogLevel.Debug)
            {
                return;
            }

            StringBuilder text = ResponseHead(request, response);

            if (level <= LogLevel.Trace)
            {
                text.AppendLine(await ReadBody(response));
            }

            logger.Log(level, text.ToString());
        }

        private Task<string> ReadBody(HttpResponse response)
        {
            return ReadContent(response.ContentType, response.Body);
        }

        private StringBuilder ResponseHead(HttpRequest request, HttpResponse response)
        {
            var status = $"{response.StatusCode} {(HttpStatusCode)response.StatusCode}";
            var text = new StringBuilder($"{request.Protocol} {status}{NewLine}");

            AppendHeaders(text, response.Headers);

            return text;
        }
    }
}