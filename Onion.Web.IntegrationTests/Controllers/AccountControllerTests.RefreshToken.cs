using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using Onion.Web.Models.Account;
using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    public partial class AccountControllerTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_RefreshToken_FailsOnMissingCookie()
        {
            // Arrange
            var client = Factory.CreateClient();
            HttpContent body = ToJsonContent("");

            // Act
            var response = await client.PostAsync("/api/account/refresh-token", body);
            var content = await ReadResponseContent<Response<AuthenticationResult>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
            content.Message.Should().Be("Cookies does not contain required refresh token.");
        }


        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_RefreshToken_GetsNewToken()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            var model = new AuthenticationRequest { Email = "superadmin@gmail.com", Password = "P@ssw0rd!" };
            HttpContent sut = ToJsonContent(model);
            HttpContent body = ToJsonContent("");

            // Act
            var authResponse = await client.PostAsync("/api/Account/Authenticate", sut);
            var refreshResponse = await client.PostAsync("/api/account/refresh-token", body);
            var authContent = await ReadResponseContent<Response<AuthenticationResult>>(authResponse);
            var refreshContent = await ReadResponseContent<Response<AuthenticationResult>>(refreshResponse);

            // Assert
            refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            refreshContent.Data.Token
                .Should().NotBeNullOrWhiteSpace()
                .And.NotBe(authContent.Data.Token);
        }
    }
}