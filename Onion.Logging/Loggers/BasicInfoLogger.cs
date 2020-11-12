using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Loggers
{
    public class BasicInfoLogger : IBasicInfoLogger
    {
        public void LogBasicInfo(ILogger logger, LogLevel level, IStopwatch stopwatch, HttpContext context)
        {
            if (level > LogLevel.Information)
            {
                return;
            }

            string method = context.Request.Method;
            string url = context.Request.GetDisplayUrl();
            var code = context.Response.StatusCode;
            var status = $"{code} {(HttpStatusCode)code}";

            logger.LogInformation($"{method} {url} at {stopwatch.Time()} with {status}");
        }
    }
}