using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Onion.Logging
{
    public class ResponseDetails
    {
        private ResponseDetails(HttpResponse response, IStopwatch stopwatch)
        {
            Stopwatch = stopwatch;
            Time = stopwatch.Time();
            StatusCode = (HttpStatusCode)response.StatusCode;
        }

        private ResponseDetails(HttpResponseMessage response, IStopwatch stopwatch)
        {
            Stopwatch = stopwatch;
            Time = stopwatch.Time();
            StatusCode = response.StatusCode;
        }

        public IStopwatch Stopwatch { get; init; }
        public string Time { get; init; }
        public HttpStatusCode StatusCode { get; init; }

        public static ResponseDetails From(HttpResponseMessage response, IStopwatch stopwatch)
            => new ResponseDetails(response, stopwatch);

        public static ResponseDetails From(HttpResponse response, IStopwatch stopwatch)
            => new ResponseDetails(response, stopwatch);
    }
}