using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;
using Onion.Logging.Middlewares;
using Onion.Logging.Tests.Helpers;
using Xunit;

namespace Onion.Logging.Tests
{
    public class ContextLoggerTests
    {
        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_Constructor_ProperlyCreatesControllerLogger()
        {
            Mock<ILoggerFactory> loggerFactoryMock = new();
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder().SetEndpoint("Name").Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.Middlewares.RequestLoggingMiddleware.Name"),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_Constructor_ProperlyCreatesMiddlewareLogger()
        {
            Mock<ILoggerFactory> loggerFactoryMock = new();
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder().Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.Middlewares.RequestLoggingMiddleware"),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task ContextLogger_LogRequest_CallsLoggerWithRequestScope()
        {
            // Arrange
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Head)
                .SetScheme(HttpScheme.Https)
                .SetHost(new("example.com"))
                .Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Act
            await sut.LogRequest(LogLevel.Trace);

            // Assert
            requestLoggerMock.Verify(
                requestLogger => requestLogger.LogRequest(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    context),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpRequest")) &&
                        scope.Contains(new KeyValuePair<string, object>("Endpoint", "https://example.com")) &&
                        scope.Contains(new KeyValuePair<string, object>("HttpMethod", "HEAD"))
                    )),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task ContextLogger_LogResponse_CallsLoggerWithRequestAndResponseScope()
        {
            // Arrange
            Mock<IStopwatch> stopwatchMock = new();
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Post)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.Conflict)
                .Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Mock
            stopwatchMock
                .SetupGet(stopwatch => stopwatch.Elapsed)
                .Returns(TimeSpan.FromSeconds(15));

            // Act
            await sut.LogResponse(LogLevel.Trace, stopwatchMock.Object);

            // Assert
            responseLoggerMock.Verify(
                requestLogger => requestLogger.LogResponse(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    context),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpRequest")) &&
                        scope.Contains(new KeyValuePair<string, object>("Endpoint", "http://example.com")) &&
                        scope.Contains(new KeyValuePair<string, object>("HttpMethod", "POST"))
                    )),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpResponse")) &&
                        scope.Contains(new KeyValuePair<string, object>("StatusCode", 409)) &&
                        scope.Contains(new KeyValuePair<string, object>("Elapsed", 15000d))
                    )),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_LogInfo_CallsLoggerWithRequestAndResponseScope()
        {
            // Arrange
            Mock<IStopwatch> stopwatchMock = new();
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.Ambiguous)
                .Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Mock
            stopwatchMock
                .SetupGet(stopwatch => stopwatch.Elapsed)
                .Returns(TimeSpan.FromMilliseconds(123));

            // Act
            sut.LogInfo(LogLevel.Trace, stopwatchMock.Object);

            // Assert
            basicLoggerMock.Verify(
                requestLogger => requestLogger.LogBasicInfo(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    stopwatchMock.Object,
                    context),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpRequest")) &&
                        scope.Contains(new KeyValuePair<string, object>("Endpoint", "http://example.com")) &&
                        scope.Contains(new KeyValuePair<string, object>("HttpMethod", "GET"))
                    )),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpResponse")) &&
                        scope.Contains(new KeyValuePair<string, object>("StatusCode", 300)) &&
                        scope.Contains(new KeyValuePair<string, object>("Elapsed", 123d))
                    )),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_LogError_CallsLoggerWithRequestResponseScope()
        {
            // Arrange
            Mock<Exception> exceptionMock = new();
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.Forbidden)
                .Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Act
            sut.LogError(exceptionMock.Object, null);

            // Assert
            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpRequest")) &&
                        scope.Contains(new KeyValuePair<string, object>("Endpoint", "http://example.com")) &&
                        scope.Contains(new KeyValuePair<string, object>("HttpMethod", "GET"))
                    )),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpResponse")) &&
                        scope.Contains(new KeyValuePair<string, object>("StatusCode", 403)) &&
                        scope.Contains(new KeyValuePair<string, object>("Elapsed", 0d))
                    )),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_LogError_CallsLoggerWithRequestAndDefaultStatusScope()
        {
            // Arrange
            Mock<IStopwatch> stopwatchMock = new();
            Mock<Exception> exceptionMock = new();
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.InternalServerError)
                .Create();

            ContextLogger sut = new(
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Mock
            stopwatchMock
                .SetupGet(stopwatch => stopwatch.Elapsed)
                .Returns(TimeSpan.FromMilliseconds(321));

            // Act
            sut.LogError(exceptionMock.Object, stopwatchMock.Object);

            // Assert
            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpRequest")) &&
                        scope.Contains(new KeyValuePair<string, object>("Endpoint", "http://example.com")) &&
                        scope.Contains(new KeyValuePair<string, object>("HttpMethod", "GET"))
                    )),
                Times.Once);

            loggerMock.Verify(
                innerLogger => innerLogger.BeginScope(
                    It.Is<Dictionary<string, object>>(scope =>
                        scope.Contains(new KeyValuePair<string, object>("EventName", "HttpResponse")) &&
                        scope.Contains(new KeyValuePair<string, object>("StatusCode", 500)) &&
                        scope.Contains(new KeyValuePair<string, object>("Elapsed", 321d))
                    )),
                Times.Once);
        }

        private static Mock<ILogger<RequestLoggingMiddleware>> LoggerMock(Mock<ILoggerFactory> loggerFactory)
        {
            Mock<ILogger<RequestLoggingMiddleware>> loggerMock = new();
            loggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);

            ILogger logger = new Logger<RequestLoggingMiddleware>(loggerFactory.Object);
            loggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(logger);

            return loggerMock;
        }
    }
}