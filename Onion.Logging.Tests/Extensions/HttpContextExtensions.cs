using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Primitives;

namespace Onion.Logging.Tests
{
    public static class HttpContextExtensions
    {
        internal static HttpContext CreateContext(
            string method = "GET",
            HttpScheme scheme = HttpScheme.Http,
            HostString? host = null,
            PathString? pathBase = null,
            PathString? path = null,
            string protocol = "HTTP/1.1",
            Dictionary<string, string>? queryParams = null,
            Dictionary<string, StringValues>? headers = null,
            Dictionary<string, StringValues>? responseHeaders = null,
            string request = "request body",
            HttpStatusCode responseStatus = HttpStatusCode.OK,
            string response = "response body")
        {
            HttpContext context = new DefaultHttpContext();

            context.Request.Method = method;
            context.Request.Scheme = scheme.ToString().ToLower();
            context.Request.Host = host ?? new("localhost");
            context.Request.PathBase = pathBase ?? "/master";
            context.Request.Path = path ?? "/slave";
            context.Request.QueryString = QueryString.Create(queryParams ?? new Dictionary<string, string>());
            context.Request.Protocol = protocol;
            context.Request.ContentType = "text/plain";

            context.Request.Body = new MemoryStream();
            byte[] bodyBytes = Encoding.UTF8.GetBytes(request);
            context.Request.Body.Write(bodyBytes, 0, request.Length);
            context.Request.Body.Position = 0;
            context.Request.Headers.AddRange(headers);

            context.Response.StatusCode = (int)responseStatus;
            context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(response));
            context.Response.ContentType = "text/plain";
            context.Response.Headers.AddRange(responseHeaders);

            return context;
        }

        private static void AddRange(this IHeaderDictionary target, IDictionary<string, StringValues>? values)
        {
            if (values is null)
            {
                return;
            }

            foreach ((string? key, StringValues value) in values)
            {
                target[key] = value;
            }
        }
    }
}