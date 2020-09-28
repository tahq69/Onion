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
        public static async Task<string> AddAuthentication<TFixture>(
            this HttpClient client,
            ControllerTest<TFixture> ctrl,
            string userEmail)
            where TFixture : InMemoryDatabaseAppFactory
        {
            var token = await ctrl.CreateToken(userEmail);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return token;
        }

        public static async Task<string> CreateToken<TFixture>(this ControllerTest<TFixture> ctrl, string userEmail)
            where TFixture : InMemoryDatabaseAppFactory
        {
            IJwtService jwt = ctrl.Factory.Services.GetRequiredService<IJwtService>();
            ApplicationUser user = await ctrl.Factory.Services
                .GetRequiredService<UserManager<ApplicationUser>>()
                .FindByEmailAsync(userEmail);

            return jwt.WriteToken(await jwt.GenerateJwtToken(user));
        }
    }
}