using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Factories
{
    /// <summary>
    /// Logging content factory to resolve body content.
    /// </summary>
    public class LogContentFactory
    {
        private readonly IEnumerable<IRequestContentLogMiddleware> _middlewares;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogContentFactory"/> class.
        /// </summary>
        /// <param name="middlewares">The middlewares.</param>
        public LogContentFactory(IEnumerable<IRequestContentLogMiddleware> middlewares)
        {
            _middlewares = middlewares ?? Enumerable.Empty<IRequestContentLogMiddleware>();
        }

        /// <summary>
        /// Prepares the body for logging.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="content">The content.</param>
        /// <returns>Prepared body.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="content"/> not provided.
        /// </exception>
        public async Task<string> PrepareBody(string contentType, Stream content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            using var ms = new MemoryStream();
            await content.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            IRequestContentLogMiddleware? middleware = _middlewares
                .FirstOrDefault(x => contentType?.Contains(x.ContentType, comparison) ?? false);

            string text = string.Empty;
            if (middleware != null && ms.CanRead)
            {
                text = middleware.Modify(ms);
            }
            else if (ms.CanRead)
            {
                using var sr = new StreamReader(ms);
                text = await sr.ReadToEndAsync();
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                return new StringBuilder()
                    .AppendLine()
                    .Append(text)
                    .ToString();
            }

            return string.Empty;
        }
    }
}