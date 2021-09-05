using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP logger factory.
    /// </summary>
    public class HttpLoggerFactory : Factory, IHttpLoggerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLoggerFactory"/> class.
        /// </summary>
        /// <param name="services">The <see cref="IServiceProvider"/> to retrieve the service objects from.</param>
        public HttpLoggerFactory(IServiceProvider services)
            : base(services)
        {
        }

        /// <inheritdoc />
        public IHttpLogger Create(ILogger logger)
        {
            return new HttpLogger(
                logger,
                GetService<IRequestLogger>(),
                GetService<IResponseLogger>(),
                GetService<IBasicInfoLogger>(),
                GetService<IEnumerable<IHttpRequestPredicate>>());
        }
    }
}