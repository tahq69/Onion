using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Onion.Logging.Middlewares;

namespace Onion.Logging
{
    internal static class LoggerFactoryExtnsions
    {
        public static ILogger ControllerLogger(this ILoggerFactory loggerFactory, HttpContext context)
        {
            var controllerName = context.ControllerName();
            if (controllerName is null)
            {
                return loggerFactory.CreateLogger<RequestLoggingMiddleware>();
            }

            var name = nameof(RequestLoggingMiddleware);
            var service = typeof(RequestLoggingMiddleware).FullName ?? name;
            return loggerFactory.CreateLogger($"{service}.{controllerName}");
        }
    }
}