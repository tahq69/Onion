using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Services
{
    /// <summary>
    /// Json content builder with custom ke/value factory methods.
    /// </summary>
    public class JsonContentBuilder : IJsonContentBuilder
    {
        private readonly Func<object?, string?> _keyFactory;
        private readonly Func<JsonToken, object?, object?> _valueFactory;

        private readonly StringBuilder _stringBuilder;
        private readonly StringWriter _stringWriter;
        private readonly JsonTextWriter _jsonWriter;


        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContentBuilder"/> class.
        /// </summary>
        public JsonContentBuilder()
            : this(null, null)
        {
        }

        private JsonContentBuilder(
            Func<object?, string?>? keyFactory,
            Func<JsonToken, object?, object?>? valueFactory)
        {
            _keyFactory = keyFactory ?? ((v) => v?.ToString());
            _valueFactory = valueFactory ?? ((t, v) => v);

            _stringBuilder = new StringBuilder();
            _stringWriter = new StringWriter(_stringBuilder);
            _jsonWriter = new JsonTextWriter(_stringWriter);
        }

        /// <inheritdoc />
        public IJsonContentBuilder Start(
            Func<object?, string?>? keyFactory = null,
            Func<JsonToken, object?, object?>? valueFactory = null)
        {
            return new JsonContentBuilder(keyFactory, valueFactory);
        }

        /// <inheritdoc/>
        public IJsonContentBuilder AddElement(JsonToken tokenType, object? value)
        {
            switch (tokenType)
            {
                case JsonToken.StartObject:
                    _jsonWriter.WriteStartObject();
                    return this;

                case JsonToken.EndObject:
                    _jsonWriter.WriteEndObject();
                    return this;

                case JsonToken.StartArray:
                    _jsonWriter.WriteStartArray();
                    return this;

                case JsonToken.EndArray:
                    _jsonWriter.WriteEndArray();
                    return this;

                default:
                    return AddToken(tokenType, value);
            }
        }

        /// <inheritdoc/>
        public string Build()
        {
            _stringWriter.Flush();

            var result = _stringBuilder.ToString();

            _stringWriter.Dispose();
            _jsonWriter.Close();

            return result;
        }

        /// <inheritdoc />
        public string Build(
            JsonTextReader reader,
            Func<object?, string?>? keyFactory = null,
            Func<JsonToken, object?, object?>? valueFactory = null)
        {
            var builder = Start(keyFactory, valueFactory);
            while (reader.Read())
            {
                builder.AddElement(reader.TokenType, reader.Value);
            }

            return builder.Build();
        }

        private IJsonContentBuilder AddToken(JsonToken tokenType, object? value)
        {
            if (tokenType == JsonToken.PropertyName)
            {
                _jsonWriter.WritePropertyName(_keyFactory(value), true);
            }
            else
            {
                var val = _valueFactory(tokenType, value);
                _jsonWriter.WriteValue(val);
            }

            return this;
        }
    }
}