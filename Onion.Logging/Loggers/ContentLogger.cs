﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Onion.Logging.Factories;

namespace Onion.Logging.Loggers
{
    /// <summary>
    /// Abstract logger with reusable methods.
    /// </summary>
    public abstract class ContentLogger
    {
        private readonly LogHeaderFactory _headerFactory;
        private readonly LogContentFactory _contentFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLogger"/> class.
        /// </summary>
        /// <param name="headerFactory">Header value middleware factory.</param>
        /// <param name="contentFactory">Content value middleware factory.</param>
        protected ContentLogger(LogHeaderFactory headerFactory, LogContentFactory contentFactory)
        {
            _headerFactory = headerFactory;
            _contentFactory = contentFactory;
        }

        /// <summary>
        /// Gets environment new line character.
        /// </summary>
        protected static string NewLine => Environment.NewLine;

        /// <summary>
        /// Reads content and formats by the request content middlewares.
        /// </summary>
        /// <param name="contentType">Content type identifier.</param>
        /// <param name="content">Content stream.</param>
        /// <returns>Formatted content as text.</returns>
        protected async Task<string> ReadContent(string contentType, Stream content)
        {
            content.Seek(0, SeekOrigin.Begin);

            string body = await _contentFactory.PrepareBody(contentType, content);

            content.Seek(0, SeekOrigin.Begin);

            return body;
        }

        /// <summary>
        /// Append header values to <paramref name="output"/> in user friendly
        /// form and parsed by the header value middlewares.
        /// </summary>
        /// <param name="output">The target to write header keys and formatted values.</param>
        /// <param name="headers">The headers collection to write.</param>
        protected void AppendHeaders(StringBuilder output, IEnumerable<KeyValuePair<string, StringValues>> headers)
        {
            foreach (var header in headers)
            {
                string key = header.Key;
                string value = _headerFactory.PrepareHeader(header);

                output.AppendLine($"{key}: {value}");
            }
        }
    }
}