using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.DTOs;
using Onion.Identity.Models;
using Onion.Web.Models.Account;
using Xunit;

namespace Onion.Web.IntegrationTests.Controllers
{
    public partial class AccountControllerTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_ForgotPassword_OkResponseOnValidEmail()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            HttpContent request = ToJsonContent(new ForgotPasswordRequest { Email = "unit-test-9@example.com" });

            // Act
            var response = await client.PostAsync("/api/account/forgot-password", request);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Succeeded.Should().BeTrue();
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_ForgotPassword_FailsOn()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            HttpContent request = ToJsonContent(new ForgotPasswordRequest { Email = "example@example.com" });

            // Act
            var response = await client.PostAsync("/api/account/forgot-password", request);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
        }
    }
}