﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
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
            Mock<ILogger> loggerMock = CreateFor(level);
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, null!, null!);

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
            Mock<ILogger> loggerMock = CreateFor(level);
            HttpContext context = new FakeHttpContextBuilder().Create();
            Mock<IStopwatch> stopwatchMock = new();
            var request = RequestDetails.From(context.Request);
            var response = ResponseDetails.From(context.Response, stopwatchMock.Object);
            ResponseLogger sut = new(new(null), new(null));
            
            // Act
            await sut.LogResponse(loggerMock.Object, request, response);

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
            Mock<ILogger> loggerMock = CreateFor(LogLevel.Debug);
            HttpContext context = new FakeHttpContextBuilder()
                .SetResponseHeaders(new() { { "foo", new(new[] { "bar", "baz" }) } })
                .Create();
            Mock<IStopwatch> stopwatchMock = new();
            var request = RequestDetails.From(context.Request);
            var response = ResponseDetails.From(context.Response, stopwatchMock.Object);
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, request, response);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}",
                LogLevel.Debug);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_TraceWritesStatusAndHeadersAndBody()
        {
            // Arrange
            Mock<ILogger> loggerMock = CreateFor(LogLevel.Trace);
            HttpContext context = new FakeHttpContextBuilder()
                .SetResponseBody("conflict response content")
                .SetResponseHeaders(new() { { "foo", new(new[] { "bar", "baz" }) } })
                .SetStatus(HttpStatusCode.Conflict)
                .Create();
            Mock<IStopwatch> stopwatchMock = new();
            var request = RequestDetails.From(context.Request);
            var response = ResponseDetails.From(context.Response, stopwatchMock.Object);
            ResponseLogger sut = new(new(null), new(null));

            // Act
            await sut.LogResponse(loggerMock.Object, request, response);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 409 Conflict{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"conflict response content{Environment.NewLine}",
                LogLevel.Trace);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_AppliesContentMiddleware()
        {
            // Arrange
            Mock<ILogger> loggerMock = CreateFor(LogLevel.Trace);
            Mock<IRequestContentLogMiddleware> contentMiddleware = new();
            var contentMiddlewares = new List<IRequestContentLogMiddleware> { contentMiddleware.Object };
            HttpContext context = new FakeHttpContextBuilder()
                .SetResponseBody("response")
                .SetResponseContentType("text/plain")
                .Create();
            Mock<IStopwatch> stopwatchMock = new();
            var request = RequestDetails.From(context.Request);
            var response = ResponseDetails.From(context.Response, stopwatchMock.Object);
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
            await sut.LogResponse(loggerMock.Object, request, response);

            // Assert
            loggerMock.VerifyLogging(
                $"HTTP/1.1 200 OK{Environment.NewLine}" +
                $"Content-Type: text/plain{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"*modified*{Environment.NewLine}",
                LogLevel.Trace);
        }

        private Mock<ILogger> CreateFor(LogLevel level)
        {
            Mock<ILogger> loggerMock = new();
            loggerMock
                .Setup(logger => logger.IsEnabled(level))
                .Returns(true);

            return loggerMock;
        }
    }
}