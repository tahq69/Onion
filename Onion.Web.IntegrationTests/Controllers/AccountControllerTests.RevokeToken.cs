using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.DTOs;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;
using Onion.Web.Models.Account;
using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    public partial class AccountControllerTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_RevokeToken_FailsOnMissingToken()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            string token = await client.AddAuthentication(this, "superadmin@gmail.com");
            HttpContent body = ToJsonContent(new RevokeTokenRequest { Token = null });

            // Act
            var response = await client.PostAsync("/api/account/revoke-token", body);
            var content = await ReadResponseContent<Response<bool>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
            content.Message.Should().Be("Token is required");
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_RevokeToken_FailsOnInvalidToken()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            string token = await client.AddAuthentication(this, "superadmin@gmail.com");
            HttpContent body = ToJsonContent(new RevokeTokenRequest { Token = "invalid" });

            // Act
            var response = await client.PostAsync("/api/account/revoke-token", body);
            var content = await ReadResponseContent<Response<bool>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Succeeded.Should().BeFalse();
            content.Message.Should().Be("Token not found");
        }


        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_RevokeToken_DeletesRefreshToken()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            string token = await client.AddAuthentication(this, "superadmin@gmail.com");
            string refreshToken = await CreateRefreshToken("superadmin@gmail.com");
            HttpContent body = ToJsonContent(new RevokeTokenRequest { Token = refreshToken });

            // Act
            var response = await client.PostAsync("/api/account/revoke-token", body);
            var content = await ReadResponseContent<Response<bool>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Succeeded.Should().BeTrue();
            content.Message.Should().Be("Token revoked");
        }

        private async Task<string> CreateRefreshToken(string userEmail)
        {
            IJwtService jwt = Factory.Services.GetRequiredService<IJwtService>();
            IUserRepository users = Factory.Services.GetRequiredService<IUserRepository>();
            ApplicationUser user = await users.SingleByEmail(userEmail);

            var token = jwt.GenerateRefreshToken(null);
            await users.AddRefreshToken(user.Id, token);

            return token.Token;
        }
    }
}