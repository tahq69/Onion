using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Loggers
{
    public class ResponseLogger : RequestLogger, IResponseLogger
    {
        private readonly LogContentFactory _contentFactory;

        public ResponseLogger(
            LogContentFactory contentFactory,
            LogHeaderFactory headerFactory)
            : base(contentFactory, headerFactory)
        {
            _contentFactory = contentFactory;
        }

        public async Task LogResponse(ILogger logger, LogLevel level, IStopwatch stopwatch, HttpContext context)
        {
            if (level > LogLevel.Debug)
            {
                return;
            }

            HttpResponse response = context.Response;
            StringBuilder text = ResponseHead(context.Request, response, stopwatch);

            if (level <= LogLevel.Trace)
            {
                text.AppendLine(await ReadBody(response));
            }

            logger.Log(level, text.ToString());
        }

        private async Task<string> ReadBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            string body = await _contentFactory.PrepareBody(response.ContentType, response.Body);

            response.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }

        private StringBuilder ResponseHead(HttpRequest request, HttpResponse response, IStopwatch stopwatch)
        {
            var status = $"{response.StatusCode} {(HttpStatusCode)response.StatusCode}";
            var text = new StringBuilder($"{request.Protocol} {status}{NewLine}");

            AppendHeaders(text, response.Headers);

            return text;
        }
    }
}