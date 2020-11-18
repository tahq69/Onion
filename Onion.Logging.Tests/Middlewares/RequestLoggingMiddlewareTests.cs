using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
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
        private HttpContext Context =>
            new FakeHttpContextBuilder("HTTP/1.1")
                .SetMethod(HttpMethod.Get)
                .SetScheme(HttpScheme.Http)
                .SetHost(new("localhost"))
                .SetPathBase("/master")
                .SetPath("/slave")
                .SetRequestHeaders(new()
                {
                    { "Authorization", "Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==" },
                    { "Foo", new StringValues("bar, baz") }
                })
                .SetRequestBody("Request body")
                .SetEndpoint("Fake")
                .Create();

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_InformationLevel()
        {
            // Arrange
            var provider = CreateServiceProvider(config => config.MinimumLevel.Information());
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
                "Information: Before { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_DebugLevel()
        {
            // Arrange
            var provider = CreateServiceProvider(config => config.MinimumLevel.Debug());
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
                "Information: Before { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }",
                @"Debug: GET http://localhost/master/slave HTTP/1.1
Host: localhost
Authorization: Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==
Foo: bar, baz
 { SourceContext: ""Onion.Logging.RequestLoggingMiddleware.Fake"", EventName: ""HttpRequest"", Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                @"Debug: HTTP/1.1 200 OK
Foo: Bar
 { SourceContext: ""Onion.Logging.RequestLoggingMiddleware.Fake"", EventName: ""HttpResponse"", StatusCode: 200, Elapsed: 100, Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_TraceLevel()
        {
            // Arrange
            var provider = CreateServiceProvider(config => config.MinimumLevel.Verbose());
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
                "Information: Before { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }",
                @"Verbose: GET http://localhost/master/slave HTTP/1.1
Host: localhost
Authorization: Basic RE9NQUlOXHVzZXJuYW1lOnBhc3N3b3JkCg==
Foo: bar, baz

Request body
 { SourceContext: ""Onion.Logging.RequestLoggingMiddleware.Fake"", EventName: ""HttpRequest"", Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                @"Verbose: HTTP/1.1 200 OK
Foo: Bar

Response body
 { SourceContext: ""Onion.Logging.RequestLoggingMiddleware.Fake"", EventName: ""HttpResponse"", StatusCode: 200, Elapsed: 100, Endpoint: ""http://localhost/master/slave"", HttpMethod: ""GET"" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 200 OK { SourceContext: \"Onion.Logging.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 200, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_ExceptionLogged()
        {
            // Arrange
            var provider = CreateServiceProvider(config => config.MinimumLevel.Information());
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
                "Information: Before { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }",
                "Error: Error during http request processing { SourceContext: \"Onion.Logging.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 500, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: GET http://localhost/master/slave at 00:00:00:100 with 500 InternalServerError { SourceContext: \"Onion.Logging.RequestLoggingMiddleware.Fake\", EventName: \"HttpResponse\", StatusCode: 500, Elapsed: 100, Endpoint: \"http://localhost/master/slave\", HttpMethod: \"GET\" }",
                "Information: After { SourceContext: \"Onion.Logging.RequestLoggingMiddleware\" }");
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_FatalNothingLogged()
        {
            // Arrange
            var provider = CreateServiceProvider(config => config.MinimumLevel.Fatal());
            ILoggerFactory factory = provider.GetService<ILoggerFactory>();
            RequestLoggingMiddlewareUnderTest handler = new(
                async ctx => await ctx.Response.WriteAsync("Response body"),
                factory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            await handler.Invoke(Context, Enumerable.Empty<IHttpRequestPredicate>());

            // Assert
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEmpty();
        }

        [Fact, Trait("Category", "Unit")]
        public async Task RequestLoggingMiddleware_Invoke_NoneIfPredicateShouldSkip()
        {
            // Arrange
            var provider = CreateServiceProvider(configuration => configuration.MinimumLevel.Verbose());
            ILoggerFactory loggerFactory = provider.GetService<ILoggerFactory>();
            EndpointPredicate predicate = new(exclude: true, patterns: new[] { "/master/*" });
            RequestLoggingMiddlewareUnderTest handler = new(
                next: async httpContext => await httpContext.Response.WriteAsync("Response body"),
                loggerFactory);

            // Act
            using var _ = TestCorrelator.CreateContext();
            await handler.Invoke(Context, new[] { predicate });

            // Assert
            List<string> actual = TestCorrelator.GetLogEventsFromCurrentContext().Select(FormatLogEvent).ToList();
            actual.Should().BeEmpty();
        }

        private static ServiceProvider CreateServiceProvider(
            Func<LoggerConfiguration, LoggerConfiguration> configureLogger)
        {
            var logger = configureLogger(new())
                .WriteTo.TestCorrelator()
                .CreateLogger();

            SerilogLoggerFactory loggerFactory = new(logger);

            return new ServiceCollection()
                .AddSingleton<ILoggerFactory>(_ => loggerFactory)
                .BuildServiceProvider();
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