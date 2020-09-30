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
        /// <inheritdoc/>
        public string Modify(string key, string value)
        {
            StringComparison comparison = StringComparison.Ordinal;

            if (key == "Authorization" && value.StartsWith("Basic ", comparison))
            {
                // Do not log passwords
                return "Basic *****";
            }

            if (key == "Authorization" && value.StartsWith("Bearer ", comparison))
            {
                // Do not log tokens
                return "Bearer *****";
            }

            return value;
        }
    }
}