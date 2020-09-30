using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Factories
{
    /// <summary>
    /// Header logging factory.
    /// </summary>
    public class LogHeaderFactory
    {
        private readonly IEnumerable<IHeaderLogMiddleware> _middlewares;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogHeaderFactory"/> class.
        /// </summary>
        /// <param name="middlewares">Header logging middlewares.</param>
        public LogHeaderFactory(IEnumerable<IHeaderLogMiddleware> middlewares)
        {
            _middlewares = middlewares;
        }

        /// <summary>
        /// Prepare header value for logging.
        /// </summary>
        /// <param name="header">Request header.</param>
        /// <returns>Logging ready value.</returns>
        public string PrepareHeader(KeyValuePair<string, StringValues> header)
        {
            var key = header.Key;
            var value = header.Value.ToString();
            foreach (IHeaderLogMiddleware middleware in _middlewares)
            {
                value = middleware.Modify(key, value);
            }

            return value;
        }
    }
}