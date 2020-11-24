using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Onion.Logging
{
    public class RequestDetails
    {
        public RequestDetails()
        {
        }

        private RequestDetails(HttpRequest request)
        {
            Method = request.Method;
            Scheme = request.Scheme;
            Host = request.Host.Value;
            Path = FluentUrl.Combine(request.PathBase, request.Path);
            QueryString = request.QueryString.Value;
            Url = request.GetDisplayUrl();
        }

        private RequestDetails(HttpRequestMessage request)
        {
            Method = request.Method.Method;
            Scheme = request.RequestUri.Scheme;
            Host = request.RequestUri.Host;
            Path = request.RequestUri.AbsolutePath;
            QueryString = request.RequestUri.Query;
            Url = request.RequestUri.ToString();
        }

        public string? Method { get; init; }
        public string? Scheme { get; init; }
        public string? Host { get; init; }
        public string? Path { get; init; }
        public string? QueryString { get; init; }
        public string? Url { get; init; }

        public static RequestDetails From(HttpRequestMessage request)
            => new RequestDetails(request);

        public static RequestDetails From(HttpRequest request)
            => new RequestDetails(request);
    }
}