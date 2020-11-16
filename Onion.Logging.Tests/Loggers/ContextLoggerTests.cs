using System.Collections.Generic;
using System.Linq;
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

            await sut.LogRequest(LogLevel.Trace);

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

        private static Mock<ILogger<RequestLoggingMiddleware>> LoggerMock(Mock<ILoggerFactory> loggerFactoryMock)
        {
            Mock<ILogger<RequestLoggingMiddleware>> loggerMock = new();

            loggerFactoryMock
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);
            ILogger logger = new Logger<RequestLoggingMiddleware>(loggerFactoryMock.Object);
            loggerFactoryMock
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(logger);

            return loggerMock;
        }
    }
}