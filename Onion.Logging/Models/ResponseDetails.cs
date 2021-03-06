﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Onion.Logging
{
    public class ResponseDetails
    {
        private ResponseDetails(IStopwatch? stopwatch)
        {
            Stopwatch = stopwatch;
            Time = stopwatch?.Time() ?? string.Empty;
        }

        private ResponseDetails(HttpResponse? response, IStopwatch? stopwatch)
            : this(stopwatch)
        {
            if (response is null)
            {
                return;
            }


            StatusCode = (HttpStatusCode)response.StatusCode;
            ContentType = response.ContentType;
            Headers = response.Headers;
            Content = response.Body;
        }

        private ResponseDetails(HttpResponseMessage? response, IStopwatch? stopwatch)
            : this(stopwatch)
        {
            if (response is null)
            {
                return;
            }

            StatusCode = response.StatusCode;
            ContentType = response.Content.Headers.ContentType.ToString();
            Headers = response.Headers.ToDictionary(
                header => header.Key,
                header => new StringValues(header.Value.ToArray()));

            Content = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
        }

        public IStopwatch? Stopwatch { get; init; }
        public string? Time { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public string? ContentType { get; init; }
        public IDictionary<string, StringValues>? Headers { get; init; }
        public Stream Content { get; init; }

        public static ResponseDetails From(HttpResponseMessage? response, IStopwatch? stopwatch) =>
            response is null ? new ResponseDetails(stopwatch) : new ResponseDetails(response, stopwatch);

        public static ResponseDetails From(HttpResponse? response, IStopwatch? stopwatch) =>
            response is null ? new ResponseDetails(stopwatch) : new ResponseDetails(response, stopwatch);
    }
}