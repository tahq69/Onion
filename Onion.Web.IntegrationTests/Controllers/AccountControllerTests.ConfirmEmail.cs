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
        public async Task AccountController_ConfirmEmail_ConfirmsUserEmail()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            ApplicationUser user = await this.FindUser("superadmin@gmail.com");
            string code = await GenerateEmailConfirmationToken(user);

            var requestUri = "/api/account/confirm-email";
            requestUri = QueryHelpers.AddQueryString(requestUri, "userId", user.Id);
            requestUri = QueryHelpers.AddQueryString(requestUri, "code", code);

            // Act
            var response = await client.GetAsync(requestUri);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Succeeded.Should().BeTrue();
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_ConfirmEmail_FailsOnInvalidCode()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            ApplicationUser user = await this.FindUser("superadmin@gmail.com");
            string code = "invalid code value";

            var requestUri = "/api/account/confirm-email";
            requestUri = QueryHelpers.AddQueryString(requestUri, "userId", user.Id);
            requestUri = QueryHelpers.AddQueryString(requestUri, "code", code);

            // Act
            var response = await client.GetAsync(requestUri);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
        }

        [Fact, Trait("Category", "Integration")]
        public async Task AccountController_ConfirmEmail_FailsOnInvalidEmail()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            ApplicationUser user = await this.FindUser("superadmin@gmail.com");
            string code = await GenerateEmailConfirmationToken(user);

            var requestUri = "/api/account/confirm-email";
            requestUri = QueryHelpers.AddQueryString(requestUri, "userId", $"invalid{user.Id}");
            requestUri = QueryHelpers.AddQueryString(requestUri, "code", code);

            // Act
            var response = await client.GetAsync(requestUri);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Succeeded.Should().BeFalse();
        }

        private async Task<string> GenerateEmailConfirmationToken(ApplicationUser user) =>
            WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(
                await Factory.Services
                    .GetRequiredService<UserManager<ApplicationUser>>()
                    .GenerateEmailConfirmationTokenAsync(user)));
    }
}