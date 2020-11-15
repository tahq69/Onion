using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;
using Xunit;

namespace Onion.Logging.Tests
{
    public class ResponseLoggerTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Information)]
        public async Task ResponseLogger_LogResponse_DoesNotWritesLogIfLevelIsNotSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, level, null);

            // Assert
            loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Never);
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        public async Task ResponseLogger_LogResponse_WritesLogIfLevelIsSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext();
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, level, context);

            // Assert
            loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_DebugWritesOnlyStatusAndHeaders()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext(
                responseHeaders: new() { { "foo", new(new[] { "bar", "baz" }) } });
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, LogLevel.Debug, context);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Content-Type: text/plain{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}",
                LogLevel.Debug);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_TraceWritesStatusAndHeadersAndBody()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext(
                responseHeaders: new() { { "foo", new(new[] { "bar", "baz" }) } },
                responseStatus: HttpStatusCode.Conflict,
                response: "conflict response content");
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, LogLevel.Trace, context);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 409 Conflict{Environment.NewLine}" +
                $"Content-Type: text/plain{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"conflict response content{Environment.NewLine}",
                LogLevel.Trace);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_AppliesContentMiddleware()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IRequestContentLogMiddleware> contentMiddleware = new();
            var contentMiddlewares = new List<IRequestContentLogMiddleware> { contentMiddleware.Object };
            HttpContext context = HttpContextExtensions.CreateContext(response: "response");
            LogContentFactory contentFactory = new(contentMiddlewares);
            Stream modifiedContent = new MemoryStream(Encoding.UTF8.GetBytes("*modified*"));
            ResponseLogger sut = new(contentFactory, new(null));

            // Mock
            contentMiddleware
                .SetupGet(middleware => middleware.ContentType)
                .Returns("text/plain");

            contentMiddleware
                .Setup(middleware => middleware.Modify(It.IsAny<Stream>(), It.IsAny<Stream>()))
                .Callback<Stream, Stream>((input, output) => modifiedContent.CopyTo(output));

            // Act
            await sut.LogResponse(loggerMock.Object, LogLevel.Trace, context);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Content-Type: text/plain{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"*modified*{Environment.NewLine}",
                LogLevel.Trace);
        }
    }
}