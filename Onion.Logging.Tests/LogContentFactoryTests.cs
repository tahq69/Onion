using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;
using Onion.Logging.Services;
using Xunit;

namespace Onion.Logging.Tests
{
    public class LogContentFactoryTests
    {
        [Fact, Trait("Category", "Unit")]
        public async Task LogContentFactory_PrepareBody_ProperlyHandlesMiddleware()
        {
            // Arrange
            var jsonBuilder = new JsonContentBuilder();
            var middlewares = new List<IRequestContentLogMiddleware> { new LongJsonContentMiddleware(jsonBuilder, 16) };
            var sut = new LogContentFactory(middlewares);
            string content = "{\"trimOn14\":\"123456789012345\",\"trimOn16\":\"12345678901234567\"}";
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            Stream stream = new MemoryStream(contentBytes);

            // Act
            string result = await sut.PrepareBody("application/json", stream);

            // Assert
            result.Should()
                .NotBeNullOrEmpty()
                .And.BeEquivalentTo(
                    $"{Environment.NewLine}{{\"trimOn14\":\"123456789012345\",\"trimOn16\":\"1234567890...\"}}");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task LogContentFactory_PrepareBody_ProperlyHandlesEmptyMiddlewares()
        {
            // Arrange
            var sut = new LogContentFactory(new List<IRequestContentLogMiddleware>());
            string content = "{\"trimOn14\":\"123456789012345\",\"trimOn16\":\"12345678901234567\"}";
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            Stream stream = new MemoryStream(contentBytes);

            // Act
            string result = await sut.PrepareBody("application/json", stream);

            // Assert
            result.Should()
                .NotBeNullOrEmpty()
                .And.BeEquivalentTo(
                    $"{Environment.NewLine}{{\"trimOn14\":\"123456789012345\",\"trimOn16\":\"12345678901234567\"}}");
        }
    }
}