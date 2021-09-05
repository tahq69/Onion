using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request details model.
    /// </summary>
    public class RequestDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDetails"/> class.
        /// </summary>
        public RequestDetails()
        {
        }

        private RequestDetails(HttpRequest? request)
        {
            if (request is null) return;

            Protocol = request.Protocol;
            Method = request.Method;
            Scheme = request.Scheme;
            Host = request.Host.Value;
            Path = FluentUrl.Combine(request.PathBase, request.Path);
            QueryString = request.QueryString.Value;
            Url = request.GetDisplayUrl();
            Headers = request.Headers;
            Content = request.Body;
            ContentType = request.ContentType;
        }

        private RequestDetails(HttpRequestMessage? request)
        {
            if (request is null) return;

            Protocol = $"HTTP/{request.Version}";
            Method = request.Method.Method;
            Scheme = request.RequestUri.Scheme;
            Host = request.RequestUri.Host;
            Path = request.RequestUri.AbsolutePath;
            QueryString = request.RequestUri.Query;
            Url = request.RequestUri.ToString();
            Headers = request.Headers.ToDictionary(
                header => header.Key,
                header => new StringValues(header.Value.ToArray()));

            Content = request.Content?.ReadAsStreamAsync().GetAwaiter().GetResult();
            ContentType = request.Content?.Headers.ContentType.ToString();
        }

        /// <summary>
        /// Gets HTTP request protocol with version.
        /// </summary>
        public string? Protocol { get; init; }

        /// <summary>
        /// Gets HTTP request method.
        /// </summary>
        public string? Method { get; init; }

        /// <summary>
        /// Gets the Scheme string of request uri.
        /// </summary>
        public string? Scheme { get; init; }

        /// <summary>
        /// Gets a hostname part (special formatting for IPv6 form) of request uri.
        /// </summary>
        public string? Host { get; init; }

        /// <summary>
        /// Gets a request uri absolute path.
        /// </summary>
        public string? Path { get; init; }

        /// <summary>
        /// Gets a request uri query string.
        /// </summary>
        public string? QueryString { get; init; }

        /// <summary>
        /// Gets a request full uri.
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// Gets HTTP request content media type.
        /// </summary>
        public string? ContentType { get; init; }

        /// <summary>
        /// Gets HTTP request headers.
        /// </summary>
        public IDictionary<string, StringValues>? Headers { get; init; }

        /// <summary>
        /// Gets HTTP request content as <see cref="Stream"/>.
        /// </summary>
        public Stream? Content { get; init; }

        /// <summary>
        /// Create request details from provided <paramref name="request"/> instance.
        /// </summary>
        /// <param name="request">The HTTP request message instance.</param>
        /// <returns>New instance of the <see cref="RequestDetails"/> model.</returns>
        public static RequestDetails From(HttpRequestMessage? request) => new(request);

        /// <summary>
        /// Create request details from provided <paramref name="request"/> instance.
        /// </summary>
        /// <param name="request">The HTTP request instance.</param>
        /// <returns>New instance of the <see cref="RequestDetails"/> model.</returns>
        public static RequestDetails From(HttpRequest? request) => new(request);
    }
}