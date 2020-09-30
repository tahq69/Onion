using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Onion.Logging.Services;
using Xunit;

namespace Onion.Logging.Tests
{
    public class EndpointPredicateTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData("/api/*", "/api/123/foo", false)]
        [InlineData("/api/*", "/api", true)]
        [InlineData("/api/*", "/health", true)]
        [InlineData("/api/*", "/", true)]
        [InlineData("/api/*", "", true)]
        public void EndpointPredicate_Filter_ExcludePatterns(string pattern, string path, bool expected)
        {
            // Arrange
            var req = Substitute.For<HttpRequest>();
            req.Path.Returns(new PathString(path));

            EndpointPredicate predicate = new EndpointPredicate(true, new[] { pattern });

            // Act
            bool skip = predicate.Filter(req);

            // Assert
            skip.Should().Be(expected);
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData("/health", "/api/123/foo", false)]
        [InlineData("/health", "/health-ui", false)]
        [InlineData("/health", "/health", true)]
        [InlineData("/health", "/", false)]
        [InlineData("/health", "", false)]
        public void EndpointPredicate_Filter_IncludePatterns(string pattern, string path, bool expected)
        {
            // Arrange
            var req = Substitute.For<HttpRequest>();
            req.Path.Returns(new PathString(path));

            EndpointPredicate predicate = new EndpointPredicate(false, new[] { pattern });

            // Act
            bool skip = predicate.Filter(req);

            // Assert
            skip.Should().Be(expected);
        }

        [Fact, Trait("Category", "Unit")]
        public void EndpointPredicate_Filter_MultipleIncludePatterns()
        {
            // Arrange
            var req = Substitute.For<HttpRequest>();
            req.Path.Returns(new PathString("/api/123/foo"));

            EndpointPredicate predicate = new EndpointPredicate(false, new[] { "/api", "/health", "/ping", "/" });

            // Act
            bool skip = predicate.Filter(req);

            // Assert
            skip.Should().BeFalse();
        }
    }
}