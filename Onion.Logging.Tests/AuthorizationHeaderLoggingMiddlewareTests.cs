using System.Threading.Tasks;
using FluentAssertions;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;
using Xunit;

namespace Onion.Logging.Tests
{
    public class AuthorizationHeaderLoggingMiddlewareTests
    {
        [Fact, Trait("Category", "Unit")]
        public void AuthorizationHeaderLoggingMiddleware_Modify_HidesBearerAuthorizationTokenValue()
        {
            // Arrange
            IHeaderLogMiddleware sut = new AuthorizationHeaderLoggingMiddleware();

            // Act
            var result = sut.Modify("Authorization", "Bearer value");

            // Assert
            result.Should().NotBeEmpty()
                .And.Be("Bearer *****");
        }

        [Fact, Trait("Category", "Unit")]
        public void AuthorizationHeaderLoggingMiddleware_Modify_HidesBasicAuthorizationTokenValue()
        {
            // Arrange
            IHeaderLogMiddleware sut = new AuthorizationHeaderLoggingMiddleware();

            // Act
            var result = sut.Modify("Authorization", "Basic value");

            // Assert
            result.Should().NotBeEmpty()
                .And.Be("Basic *****");
        }
        
        [Fact, Trait("Category", "Unit")]
        public void AuthorizationHeaderLoggingMiddleware_Modify_IgnoresDigestValue()
        {
            // Arrange
            IHeaderLogMiddleware sut = new AuthorizationHeaderLoggingMiddleware();

            // Act
            var result = sut.Modify("Authorization", "Digest value");

            // Assert
            result.Should().NotBeEmpty()
                .And.Be("Digest value");
        }
    }
}