using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Onion.Application.DTOs;
using Onion.Web.Models.Account;
using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    public partial class AccountControllerTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_Register_FailsOnInvalidModel()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            HttpContent body = ToJsonContent(new { name = "none" });

            // Act
            var response = await client.PostAsync("/api/account/register", body);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_Register_CreatesUser()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            string username = Guid.NewGuid().ToString();
            HttpContent body = ToJsonContent(new RegisterRequest
            {
                FirstName = "FirstName",
                Email = "test@example.com",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                LastName = "LastName",
                UserName = username,
            });

            // Act
            var response = await client.PostAsync("/api/account/register", body);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, content.Message);
            content.Succeeded.Should().BeTrue(content.Message);
        }
    }
}