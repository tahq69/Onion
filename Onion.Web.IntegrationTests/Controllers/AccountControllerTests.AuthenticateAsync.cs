using System.Collections.Generic;
using System.Linq;
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
        public async Task AccountController_AuthenticateAsync_FailsOnInvalidModel()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            HttpContent sut = ToJsonContent(new { email = "value" });

            // Act
            var response = await client.PostAsync("/api/Account/Authenticate", sut);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
            content.Errors.Should().BeEquivalentTo(new Dictionary<string, ICollection<string>>
            {
                { "Email", new[] { "The Email field is not a valid e-mail address." } },
                { "Password", new[] { "The Password field is required." } },
            });
        }


        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_AuthenticateAsync_FailsOnInvalidCredentials()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            var model = new AuthenticationRequest { Email = "example@example.com", Password = "invalid password" };
            HttpContent sut = ToJsonContent(model);

            // Act
            var response = await client.PostAsync("/api/Account/Authenticate", sut);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            content.Succeeded.Should().BeFalse();
            content.Errors.Should().BeEquivalentTo(new Dictionary<string, ICollection<string>>
            {
                { "Email", new[] { "Invalid credentials provided." } },
            });
        }


        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_AuthenticateAsync_FailsOnInvalidPassword()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            var model = new AuthenticationRequest { Email = "superadmin@gmail.com", Password = "invalid password" };
            HttpContent sut = ToJsonContent(model);

            // Act
            var response = await client.PostAsync("/api/Account/Authenticate", sut);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            content.Succeeded.Should().BeFalse();
            content.Errors.Should().BeEquivalentTo(new Dictionary<string, ICollection<string>>
            {
                { "Email", new[] { "Invalid credentials provided." } },
            });
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_AuthenticateAsync_ReturnsTokenOnValidCredentials()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            var model = new AuthenticationRequest { Email = "superadmin@gmail.com", Password = "P@ssw0rd!" };
            HttpContent sut = ToJsonContent(model);

            // Act
            var response = await client.PostAsync("/api/Account/Authenticate", sut);
            var content = await ReadResponseContent<Response<AuthenticationResult>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Succeeded.Should().BeTrue();
            content.Errors.Should().BeNull();
            content.Data.Token.Should().NotBeNullOrWhiteSpace();
            content.Data.Email.Should().Be("superadmin@gmail.com");
            content.Data.UserName.Should().Be("superadmin");
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_AuthenticateAsync_SetsTokenToCookiesOnValidCredentials()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            var model = new AuthenticationRequest { Email = "superadmin@gmail.com", Password = "P@ssw0rd!" };
            HttpContent sut = ToJsonContent(model);

            // Act
            var response = await client.PostAsync("/api/Account/Authenticate", sut);
            var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cookies.Any(x => x.StartsWith("refreshToken=")).Should().BeTrue();
        }
    }
}