using Microsoft.AspNetCore.Http;
using Onion.Application.Interfaces;
using System.Security.Claims;

namespace Onion.Web.Services
{
    /// <summary>
    /// Authenticated user service implementation.
    /// </summary>
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        /// <summary>
        /// Initializes new instance of the <see cref="AuthenticatedUserService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor.</param>
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
        }

        /// <summary>
        /// Authenticated user identifier.
        /// </summary>
        public string UserId { get; }
    }
}