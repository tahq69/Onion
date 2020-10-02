using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;
using Onion.Logging.Services;

namespace Onion.Logging
{
    /// <summary>
    /// Logging service DI extensions.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds logging DI.
        /// </summary>
        /// <param name="services">DI service.</param>
        /// <param name="config">Application configuration.</param>
        public static void AddLogging(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IHeaderLogMiddleware, AuthorizationHeaderLoggingMiddleware>();

            // TODO: add LongJsonContentSettings and initialize max length from this config.
            services.AddTransient<IRequestContentLogMiddleware, LongJsonContentMiddleware>();
            services.AddTransient<IJsonStreamModifier, JsonStreamModifier>();
        }

        /// <summary>
        /// Adds endpoint patterns to ignore them from logging handler.
        /// </summary>
        /// <param name="services">DI service.</param>
        /// <param name="ignore">Collection of the endpoints to be ignored.</param>
        /// <example>
        /// <code>
        ///    services.AddLogEndpointIgnorePredicate(new [] { "/api/swagger*", "/api/healthchecks*" })
        /// </code>
        /// Will exclude all request logs to /api/swagger* and /api/healthchecks*.
        /// </example>
        public static void AddLogEndpointIgnorePredicate(
            this IServiceCollection services,
            IEnumerable<string> ignore)
        {
            services.AddSingleton<IHttpRequestPredicate>(provider =>
                new EndpointPredicate(false, ignore));
        }

        /// <summary>
        /// Adds custom HTTP request logging middleware.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static void UseLogging(IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}