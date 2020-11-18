﻿using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request basic information logger implementation.
    /// </summary>
    public class BasicInfoLogger : IBasicInfoLogger
    {
        /// <inheritdoc cref="IBasicInfoLogger"/>
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
            var message = $"{method} {url} at {stopwatch.Time()} with {status}";

            logger.LogInformation(message);
        }
    }
}