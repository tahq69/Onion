using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Onion.Logging.Factories;
using Onion.Logging.Interfaces;
using Onion.Logging.Loggers;
using Onion.Logging.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Display;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace Onion.Logging.Tests
{
    public class RequestLoggingMiddlewareTests
    {
        private HttpContext Context
        {
            get
            {
                var context = new DefaultHttpContext();
                context.Request.Method = "GET";
                context.Request.Scheme = "http";
                context.Request.Host = new HostString("localhost");
                context.Request.PathBase = "/master";
                context.Request.Path = "/slave";
                context.Request.Protocol = "HTTP/1.1";
                context.Request.Headers["Host"] = "localhost";
                context.Request.Headers["Authorization"] = "Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==";
                context.Request.Headers["Foo"] = new StringValues("bar, baz");
                context.Request.Body = new MemoryStream();
                byte[] body = Encoding.UTF8.GetBytes("Request body");
                context.Request.Body.Write(body, 0, body.Length);
                context.Request.Body.Position = 0;
                context.SetEndpoint(new Endpoint(
                    c => Task.CompletedTask,
                    new EndpointMetadataCollection(new ControllerActionDescriptor { ControllerName = "Fake" }),
                    "FakeControllerDisplayName"));

                return context;
            }
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_InformationLevel()
        {
            // Arrange
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(i => new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.TestCorrelator()
                    .CreateLogger()
                ))
                .BuildServiceProvider();

            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            ILogger<RequestLoggingMiddleware> logger = factory.CreateLogger<RequestLoggingMiddleware>();

            logger.IsEnabled(LogLevel.Error).Should().BeTrue();
            logger.IsEnabled(LogLevel.Information).Should().BeTrue();
            logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
            logger.IsEnabled(LogLevel.Trace).Should().BeFalse();

            var handler = new RequestLoggingMiddlewareUnderTest(
                async ctx =>
                {
                    ctx.Response.StatusCode = 200;
                    await ctx.Response.WriteAsync("Response body");
                },
                factory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            logger.LogInformation("Before");
            await handler.Invoke(Context, Enumerable.Empty<IHttpRequestPredicate>());
            logger.LogInformation("After");

            // Assert
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEquivalentTo(
                "Information: Before { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_DebugLevel()
        {
            // Arrange
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(i => new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.TestCorrelator()
                    .CreateLogger()
                ))
                .BuildServiceProvider();

            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            ILogger<RequestLoggingMiddleware> logger = factory.CreateLogger<RequestLoggingMiddleware>();

            logger.IsEnabled(LogLevel.Error).Should().BeTrue();
            logger.IsEnabled(LogLevel.Information).Should().BeTrue();
            logger.IsEnabled(LogLevel.Debug).Should().BeTrue();
            logger.IsEnabled(LogLevel.Trace).Should().BeFalse();

            RequestLoggingMiddlewareUnderTest handler = new(
                async ctx =>
                {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Headers["Foo"] = "Bar";
                    await ctx.Response.WriteAsync("Response body");
                },
                factory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            logger.LogInformation("Before");
            await handler.Invoke(Context, Enumerable.Empty<IHttpRequestPredicate>());
            logger.LogInformation("After");

            // Assert
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEquivalentTo(
                "Information: Before { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }",
                @"Debug: GET http://localhost/master/slave HTTP/1.1
Host: localhost
Authorization: Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==
Foo: bar, baz
 { SourceContext: ""Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake"", EventName: ""HttpRequest"", Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                @"Debug: HTTP/1.1 200 OK
Foo: Bar
 { SourceContext: ""Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake"", EventName: ""HttpResponse"", StatusCode: 200, Elapsed: 100, Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_TraceLevel()
        {
            // Arrange
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(i => new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.TestCorrelator()
                    .CreateLogger()
                ))
                .BuildServiceProvider();

            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            ILogger<RequestLoggingMiddleware> logger = factory.CreateLogger<RequestLoggingMiddleware>();

            logger.IsEnabled(LogLevel.Error).Should().BeTrue();
            logger.IsEnabled(LogLevel.Information).Should().BeTrue();
            logger.IsEnabled(LogLevel.Debug).Should().BeTrue();
            logger.IsEnabled(LogLevel.Trace).Should().BeTrue();

            RequestLoggingMiddlewareUnderTest handler = new(
                async ctx =>
                {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Headers["Foo"] = "Bar";
                    await ctx.Response.WriteAsync("Response body");
                },
                factory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            logger.LogInformation("Before");
            await handler.Invoke(Context, Enumerable.Empty<IHttpRequestPredicate>());
            logger.LogInformation("After");

            // Assert
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEquivalentTo(
                "Information: Before { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }",
                @"Verbose: GET http://localhost/master/slave HTTP/1.1
Host: localhost
Authorization: Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==
Foo: bar, baz

Request body
 { SourceContext: ""Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake"", EventName: ""HttpRequest"", Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                @"Verbose: HTTP/1.1 200 OK
Foo: Bar

Response body
 { SourceContext: ""Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake"", EventName: ""HttpResponse"", StatusCode: 200, Elapsed: 100, Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_ExceptionLogged()
        {
            // Arrange
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(i => new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.TestCorrelator()
                    .CreateLogger()
                ))
                .BuildServiceProvider();

            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            ILogger<RequestLoggingMiddleware> logger = factory.CreateLogger<RequestLoggingMiddleware>();

            logger.IsEnabled(LogLevel.Error).Should().BeTrue();
            logger.IsEnabled(LogLevel.Information).Should().BeTrue();
            logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
            logger.IsEnabled(LogLevel.Trace).Should().BeFalse();

            RequestLoggingMiddlewareUnderTest handler = new(
                ctx =>
                {
                    ctx.Response.StatusCode = 500;
                    throw new Exception("Exception message");
                },
                factory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            logger.LogInformation("Before");
            Exception ex1 = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await handler.Invoke(Context, Enumerable.Empty<IHttpRequestPredicate>());
            });
            logger.LogInformation("After");

            // Assert
            ex1.Message.Should().Be("Exception message");
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEquivalentTo(
                "Information: Before { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }",
                "Error: Error during http request processing { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 500, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 500 InternalServerError { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 500, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.Middlewares.RequestLoggingMiddleware\" }");
        }

        private string FormatLogEvent(LogEvent evt)
        {
            const string template = "{Level}: {Message:lj} {Properties}";

            var culture = CultureInfo.InvariantCulture;
            MessageTemplateTextFormatter formatter = new(template, culture);
            StringWriter sw = new();
            formatter.Format(evt, sw);

            return sw.ToString();
        }


        private class RequestLoggingMiddlewareUnderTest : RequestLoggingMiddleware
        {
            public RequestLoggingMiddlewareUnderTest(RequestDelegate next, ILoggerFactory factory)
                : base(
                    next,
                    new ContextLoggerFactory(
                        factory,
                        new RequestLogger(new(null), new(null)),
                        new ResponseLogger(new(null), new(null)),
                        new BasicInfoLogger()))
            {
            }

            protected override IStopwatch CreateStopwatch()
            {
                return new MockStopwatch(TimeSpan.FromMilliseconds(100));
            }
        }
    }
}