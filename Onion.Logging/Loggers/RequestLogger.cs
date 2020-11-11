using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;

namespace Onion.Logging.Loggers
{
    /// <summary>
    /// HTTP request logger implementation.
    /// </summary>
    public class RequestLogger : IRequestLogger
    {
        private readonly LogContentFactory _contentFactory;
        private readonly LogHeaderFactory _headerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLogger"/> class.
        /// </summary>
        /// <param name="contentFactory">Request content factory.</param>
        /// <param name="headerFactory">Request header factory.</param>
        public RequestLogger(
            LogContentFactory contentFactory,
            LogHeaderFactory headerFactory)
        {
            _contentFactory = contentFactory;
            _headerFactory = headerFactory;
        }

        /// <summary>
        /// Gets environment new line character.
        /// </summary>
        protected static string NewLine => Environment.NewLine;

        /// <inheritdoc/>
        public async Task LogRequest(ILogger logger, LogLevel level, HttpContext context)
        {
            if (level > LogLevel.Debug)
            {
                return;
            }

            HttpRequest request = context.Request;
            StringBuilder text = RequestHead(request);

            if (level <= LogLevel.Trace)
            {
                text.AppendLine(await ReadBody(request));
            }

            logger.Log(level, text.ToString());
        }

        protected void AppendHeaders(StringBuilder output, IEnumerable<KeyValuePair<string, StringValues>> headers)
        {
            foreach (var header in headers)
            {
                string key = header.Key;
                string value = _headerFactory.PrepareHeader(header);

                output.AppendLine($"{key}: {value}");
            }
        }

        private StringBuilder RequestHead(HttpRequest request)
        {
            string displayUrl = $"{request.Method} {request.GetDisplayUrl()}";

            var text = new StringBuilder($"{displayUrl} {request.Protocol}{NewLine}");
            AppendHeaders(text, request.Headers);

            return text;
        }

        private async Task<string> ReadBody(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Position = 0;

            string body = await _contentFactory.PrepareBody(request.ContentType, request.Body);

            request.Body.Position = 0;

            return body;
        }
    }
}