using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Web.IntegrationTests
{
    public static class TestExtensions
    {
        public static async Task<string> AddAuthentication<T>(
            this HttpClient client,
            ControllerTest<T> ctrl,
            string userEmail)
            where T : InMemoryDatabaseAppFactory
        {
            var token = await ctrl.CreateToken(userEmail);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return token;
        }

        public static async Task<string> CreateToken<T>(this ControllerTest<T> ctrl, string userEmail)
            where T : InMemoryDatabaseAppFactory
        {
            IJwtService jwt = ctrl.Factory.Services.GetRequiredService<IJwtService>();
            ApplicationUser user = await ctrl.FindUser(userEmail);

            return jwt.WriteToken(await jwt.GenerateJwtToken(user));
        }

        public static Task<ApplicationUser> FindUser<T>(this ControllerTest<T> ctrl, string userEmail)
            where T : InMemoryDatabaseAppFactory =>
            ctrl.Factory.Services
                .GetRequiredService<UserManager<ApplicationUser>>()
                .FindByEmailAsync(userEmail);
    }
}