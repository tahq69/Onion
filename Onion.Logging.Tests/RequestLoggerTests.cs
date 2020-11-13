﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;
using Xunit;

namespace Onion.Logging.Tests
{
    public class RequestLoggerTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Information)]
        public async Task RequestLogger_LogRequest_DoesNotWritesLogIfLevelIsNotSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext();
            RequestLogger sut = new(new(null), new(null));

            // Act
            await sut.LogRequest(loggerMock.Object, level, context);

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
        public async Task RequestLogger_LogRequest_WritesLogIfLevelIsSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext();
            RequestLogger sut = new(new(null), new(null));

            // Act
            await sut.LogRequest(loggerMock.Object, level, context);

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
        public async Task RequestLogger_LogRequest_DebugWritesOnlyUrlAndHeaders()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext(
                method: "POST",
                scheme: HttpScheme.Https,
                queryParams: new() { { "foo", "bar" } },
                headers: new() { { "foo", new(new[] { "bar", "baz" }) } });
            RequestLogger sut = new(new(null), new(null));

            // Act
            await sut.LogRequest(loggerMock.Object, LogLevel.Debug, context);

            // Assert
            loggerMock.VerifyLogging(
                $"POST https://localhost/master/slave?foo=bar HTTP/1.1{Environment.NewLine}" +
                $"Host: localhost{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}",
                LogLevel.Debug);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_TraceWritesUrlAndHeadersAndBody()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            HttpContext context = HttpContextExtensions.CreateContext(
                method: "POST",
                scheme: HttpScheme.Https,
                queryParams: new() { { "foo", "bar" } },
                headers: new() { { "foo", new(new[] { "bar", "baz" }) } },
                request: "request content string");
            RequestLogger sut = new(new(null), new(null));

            // Act
            await sut.LogRequest(loggerMock.Object, LogLevel.Trace, context);

            // Assert
            loggerMock.VerifyLogging(
                $"POST https://localhost/master/slave?foo=bar HTTP/1.1{Environment.NewLine}" +
                $"Host: localhost{Environment.NewLine}" +
                $"foo: bar,baz{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"request content string{Environment.NewLine}",
                LogLevel.Trace);
        }


        [Fact, Trait("Category", "Unit")]
        public async Task RequestLogger_LogRequest_AppliesHeaderMiddleware()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IHeaderLogMiddleware> headerMiddlewareMock = new();
            var headerMiddlewares = new List<IHeaderLogMiddleware> { headerMiddlewareMock.Object };
            LogHeaderFactory headerFactory = new(headerMiddlewares);
            HttpContext context = HttpContextExtensions.CreateContext(
                headers: new() { { "foo", new(new[] { "bar", "baz" }) } });

            RequestLogger sut = new(new(null), headerFactory);

            // Mock
            headerMiddlewareMock
                .Setup(middleware => middleware.Modify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((key, value) => value);

            headerMiddlewareMock
                .Setup(middleware => middleware.Modify("foo", It.IsAny<string>()))
                .Returns("*modified value*");

            // Act
            await sut.LogRequest(loggerMock.Object, LogLevel.Debug, context);

            // Assert
            loggerMock.VerifyLogging(
                $"GET http://localhost/master/slave HTTP/1.1{Environment.NewLine}" +
                $"Host: localhost{Environment.NewLine}" +
                $"foo: *modified value*{Environment.NewLine}",
                LogLevel.Debug);
        }
    }
}