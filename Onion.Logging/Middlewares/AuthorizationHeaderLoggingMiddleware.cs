using System;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Middlewares
{
    /// <summary>
    /// Authorization header value middleware.
    /// </summary>
    /// <remarks>
    /// Replaces Basic and Bearer authorization values with asterisk.
    /// </remarks>
    public class AuthorizationHeaderLoggingMiddleware : IHeaderLogMiddleware
    {
        private const StringComparison Comparison = StringComparison.Ordinal;

        /// <inheritdoc/>
        public string Modify(string key, string value) =>
            key switch
            {
                "Authorization" when value.StartsWith("Basic ", Comparison) => "Basic *****",
                "Authorization" when value.StartsWith("Bearer ", Comparison) => "Bearer *****",
                _ => value
            };
    }
}