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
            Mock<IHttpLoggerFactory> httpLoggerFactoryMock = new();
            HttpContext context = new FakeHttpContextBuilder().SetEndpoint("Name").Create();

            ContextLogger<RequestLoggingMiddleware> sut = new(
                loggerFactoryMock.Object,
                httpLoggerFactoryMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.RequestLoggingMiddleware.Name"),
                Times.Once);
        }

        [Fact, Trait("Category", "Unit")]
        public void ContextLogger_Constructor_ProperlyCreatesMiddlewareLogger()
        {
            Mock<ILoggerFactory> loggerFactoryMock = new();
            Mock<IHttpLoggerFactory> httpLoggerFactoryMock = new();
            HttpContext context = new FakeHttpContextBuilder().Create();

            ContextLogger<RequestLoggingMiddleware> sut = new(
                loggerFactoryMock.Object,
                httpLoggerFactoryMock.Object,
                context);

            loggerFactoryMock.Verify(
                factory => factory.CreateLogger("Onion.Logging.RequestLoggingMiddleware"),
                Times.Once);
        }
    }
}