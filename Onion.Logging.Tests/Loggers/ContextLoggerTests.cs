using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using Moq;
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

            ContextLogger<RequestLoggingMiddleware> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(), 
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.RequestLoggingMiddleware.Name"),
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

            ContextLogger<RequestLoggingMiddleware> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.RequestLoggingMiddleware"),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task ContextLogger_LogRequest_CallsLoggerWithRequestScope()
        {
            // Arrange
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock, LogLevel.Trace);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Head)
                .SetScheme(HttpScheme.Https)
                .SetHost(new("example.com"))
                .Create();

            ContextLogger<int> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Act
            await sut.LogRequest();

            // Assert
            requestLoggerMock.Verify(
                requestLogger => requestLogger.LogRequest(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    context.Request),
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
            var loggerMock = LoggerMock(loggerFactoryMock, LogLevel.Trace);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Post)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.Conflict)
                .Create();

            ContextLogger<int> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
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
            await sut.LogResponse(stopwatchMock.Object);

            // Assert
            responseLoggerMock.Verify(
                requestLogger => requestLogger.LogResponse(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    context.Request,
                    context.Response),
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
            var loggerMock = LoggerMock(loggerFactoryMock, LogLevel.Trace);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetStatus(HttpStatusCode.Ambiguous)
                .Create();

            ContextLogger<int> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
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
            sut.LogInfo(stopwatchMock.Object);

            // Assert
            basicLoggerMock.Verify(
                requestLogger => requestLogger.LogBasicInfo(
                    It.IsAny<ILogger>(),
                    LogLevel.Trace,
                    It.IsAny<RequestDetails>(),
                    It.IsAny<ResponseDetails>()),
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

            ContextLogger<int> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
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

            ContextLogger<int> sut = new(
                Enumerable.Empty<IHttpRequestPredicate>(),
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

        [Fact, Trait("Category", "Unit")]
        public async Task ContextLogger_LogResponse_NoLogsIfPredicateShouldSkip()
        {
            // Arrange
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetPathBase("/master/api/v1")
                .SetStatus(HttpStatusCode.InternalServerError)
                .Create();
            EndpointPredicate predicate = new(exclude: true, patterns: new[] { "/master/*" });
            ContextLogger<int> sut = new(
                new[] { predicate },
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Act
            await sut.LogResponse(new Mock<IStopwatch>().Object);

            // Assert
            loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task ContextLogger_LogRequest_NoLogsIfPredicateShouldSkip()
        {
            // Arrange
            Mock<ILoggerFactory> loggerFactoryMock = new();
            var loggerMock = LoggerMock(loggerFactoryMock);
            Mock<IRequestLogger> requestLoggerMock = new();
            Mock<IResponseLogger> responseLoggerMock = new();
            Mock<IBasicInfoLogger> basicLoggerMock = new();
            HttpContext context = new FakeHttpContextBuilder()
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("example.com"))
                .SetPathBase("/master/api/v1")
                .SetStatus(HttpStatusCode.InternalServerError)
                .Create();
            EndpointPredicate predicate = new(exclude: true, patterns: new[] { "/master/*" });
            ContextLogger<int> sut = new(
                new[] { predicate },
                loggerFactoryMock.Object,
                requestLoggerMock.Object,
                responseLoggerMock.Object,
                basicLoggerMock.Object,
                context);

            // Act
            await sut.LogRequest();

            // Assert
            loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        private static Mock<ILogger<RequestLoggingMiddleware>> LoggerMock(
            Mock<ILoggerFactory> loggerFactory,
            LogLevel logLevel = LogLevel.Information)
        {
            Mock<ILogger<RequestLoggingMiddleware>> loggerMock = new();
            loggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);

            ILogger innerLogger = new Logger<RequestLoggingMiddleware>(loggerFactory.Object);
            loggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(innerLogger);

            loggerMock
                .Setup(logger => logger.IsEnabled(logLevel))
                .Returns(true);

            return loggerMock;
        }
    }
}