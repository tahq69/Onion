using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Onion.Logging
{
    internal static class HttpContextExtensions
    {
        public static string? ControllerName(this HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            return descriptor?.ControllerName;
        }
    }
}