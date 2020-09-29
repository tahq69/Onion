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
        public async Task AccountController_ResetPassword_OkOnValidCodeAndEmail()
        {
            // Arrange
            HttpClient client = Factory.CreateClient();
            ApplicationUser user = await this.FindUser("superadmin@gmail.com");
            string token = await GeneratePasswordChangeToken(user);
            HttpContent request = ToJsonContent(new ResetPasswordRequest
            {
                Email = "superadmin@gmail.com",
                Password = "P@ssw0rd!",
                ConfirmPassword = "P@ssw0rd!",
                Token = token,
            });

            // Act
            var response = await client.PostAsync("/api/account/reset-password", request);
            var content = await ReadResponseContent<Response<string>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Succeeded.Should().BeTrue();
        }

        private Task<string> GeneratePasswordChangeToken(ApplicationUser user) =>
            Factory.Services
                .GetRequiredService<UserManager<ApplicationUser>>()
                .GeneratePasswordResetTokenAsync(user);
    }
}