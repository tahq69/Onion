﻿using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Onion.Logging.Interfaces;
using Onion.Logging.Middlewares;
using Onion.Tests;
using Xunit;

namespace Onion.Logging.Tests
{
    public class LongJsonContentMiddlewareTests
    {
        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_ProperlyCreateComplex()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware(50);
            string content = this.LoadResource("sis_vehicle_response.json");
            content = Regex.Replace(content, @"\s", "");
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            string expected = this.LoadResource("sis_vehicle_response_log.json");
            expected = Regex.Replace(expected, @"\s", "");
            result
                .Should().NotBeEmpty()
                .And.BeEquivalentTo(expected);
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_ProperlyCreateArray()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "[\"1\",\"2\"]";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo("[\"1\",\"2\"]");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_ProperlyCreateObject()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "{\"1\":1,\"2\":true}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo("{\"1\":1,\"2\":true}");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_ProperlyHandlesNullValue()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "{\"1\":null,\"2\":true}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo("{\"1\":null,\"2\":true}");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_ProperlyCreateNested()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "{\"1\":1.1,\"object\":[\"1\",{\"2\":null}]}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo("{\"1\":1.1,\"object\":[\"1\",{\"2\":null}]}");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_IgnoresNonJsonContentType()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "This message is not a JSON string";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo("This message is not a JSON string");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_IgnoresNonJson()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware();
            string content = "This message is not a JSON string";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should().Be("This message is not a JSON string");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_DoesNotModifySmallJson()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware(15);
            string content = @"{""key1"":1,""key2"":""some content""}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(@"{""key1"":1,""key2"":""some content""}");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_TrimsValue()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware(10);
            string content = @"{""key1"":""short"",""key2"":""some long content""}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(@"{""key1"":""short"",""key2"":""some long ...""}");
        }

        [Fact, Trait("Category", "Unit")]
        public void LongJsonContentMiddleware_Modify_TrimsValueOnMultiDimension()
        {
            // Arrange
            IRequestContentLogMiddleware sut = new LongJsonContentMiddleware(10);
            string content = @"{""key1"":""short"",""key2"":{""key1"":""some long content""}}";
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            // Act
            string result = sut.Modify(new MemoryStream(bytes));

            // Assert
            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(@"{""key1"":""short"",""key2"":{""key1"":""some long ...""}}");
        }
    }
}