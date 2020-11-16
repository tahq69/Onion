﻿using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;
using Onion.Logging.Tests.Helpers;
using Xunit;

namespace Onion.Logging.Tests
{
    public class BasicInfoLoggerTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Warning)]
        public void BasicInfoLogger_LogBasicInfo_DoesNotWritesLogIfLevelIsNotSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IStopwatch> stopwatchMock = new();
            HttpContext context = new FakeHttpContextBuilder().Create();
            BasicInfoLogger sut = new();

            // Act
            sut.LogBasicInfo(loggerMock.Object, level, stopwatchMock.Object, context);

            // Assert
            loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Never());
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Trace)]
        public void BasicInfoLogger_LogBasicInfo_WritesLogIfLevelIsSufficient(LogLevel level)
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IStopwatch> stopwatchMock = new();
            HttpContext context = new FakeHttpContextBuilder().Create();
            BasicInfoLogger sut = new();

            // Act
            sut.LogBasicInfo(loggerMock.Object, level, stopwatchMock.Object, context);

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
        public void BasicInfoLogger_LogBasicInfo_WritesSuccessfulPostRequestDetails()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IStopwatch> stopwatchMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetRequest(
                    HttpMethod.Post,
                    HttpScheme.Https,
                    new() { { "cat", "221" } })
                .Create();

            BasicInfoLogger sut = new();

            // Mock
            stopwatchMock.SetupGet(stopwatch => stopwatch.Elapsed).Returns(TimeSpan.FromSeconds(2));

            // Act
            sut.LogBasicInfo(loggerMock.Object, LogLevel.Trace, stopwatchMock.Object, context);

            // Assert
            loggerMock.VerifyLogging(
                "POST https://localhost/master/slave?cat=221 at 00:00:02:000 with 200 OK",
                LogLevel.Information);
        }

        [Fact, Trait("Category", "Unit")]
        public void BasicInfoLogger_LogBasicInfo_WritesErrorPostRequestDetails()
        {
            // Arrange
            Mock<ILogger> loggerMock = new();
            Mock<IStopwatch> stopwatchMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Post)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetPathBase("/primary")
                .SetPath("/secondary")
                .SetQuery(new() { { "cats", "1" } })
                .SetStatus(HttpStatusCode.InternalServerError)
                .Create();

            BasicInfoLogger sut = new();

            // Mock
            stopwatchMock.SetupGet(stopwatch => stopwatch.Elapsed).Returns(TimeSpan.FromSeconds(3));

            // Act
            sut.LogBasicInfo(loggerMock.Object, LogLevel.Debug, stopwatchMock.Object, context);

            // Assert
            loggerMock.VerifyLogging(
                "POST http://example.com/primary/secondary?cats=1 at 00:00:03:000 with 500 InternalServerError",
                LogLevel.Information);
        }
    }
}