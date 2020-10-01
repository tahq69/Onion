using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Onion.Logging.Interfaces;
using Onion.Logging.Services;
using Xunit;

namespace Onion.Logging.Tests
{
    public class JsonContentBuilderTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData("")]
        [InlineData("[]")]
        [InlineData("[1,2]")]
        [InlineData("{\"key\":1}")]
        [InlineData("{\"key\":true}")]
        [InlineData("{\"key\":1.2}")]
        [InlineData("{\"key\":\"value\"}")]
        public void JsonContentBuilder_Build_WritesAsOriginal(string data)
        {
            IJsonContentBuilder sut = new JsonContentBuilder();
            using var sr = new StringReader(data);
            using var jtr = new JsonTextReader(sr);
            sut.Build(jtr)
                .Should()
                .Be(data);
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData("{\"key\":1}")]
        [InlineData("{\"key\":true}")]
        [InlineData("{\"key\":1.2}")]
        [InlineData("{\"key\":\"value\"}")]
        public void JsonContentBuilder_Build_CustomValue(string data)
        {
            IJsonContentBuilder sut = new JsonContentBuilder();
            using var sr = new StringReader(data);
            using var jtr = new JsonTextReader(sr);
            sut.Build(jtr, null, (t, v) => "value")
                .Should()
                .Be("{\"key\":\"value\"}");
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData("{\"a\":\"value\"}")]
        [InlineData("{\"b\":\"value\"}")]
        [InlineData("{\"c\":\"value\"}")]
        [InlineData("{1:\"value\"}")]
        public void JsonContentBuilder_Build_CustomKey(string data)
        {
            IJsonContentBuilder sut = new JsonContentBuilder();
            using var sr = new StringReader(data);
            using var jtr = new JsonTextReader(sr);
            sut.Build(jtr, (k) => "key")
                .Should()
                .Be("{\"key\":\"value\"}");
        }
    }
}