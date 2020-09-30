using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Middlewares
{
    /// <summary>
    /// JSON content middleware. Trims property values if it exceeds max value.
    /// </summary>
    public class LongJsonContentMiddleware : IRequestContentLogMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LongJsonContentMiddleware"/> class.
        /// </summary>
        public LongJsonContentMiddleware()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LongJsonContentMiddleware"/> class.
        /// </summary>
        /// <param name="maxCharCountInField">The maximum character count in field.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="maxCharCountInField"/> is less than 1.
        /// </exception>
        public LongJsonContentMiddleware(int maxCharCountInField)
        {
            if (maxCharCountInField < 1)
                throw new ArgumentOutOfRangeException(nameof(maxCharCountInField));

            MaxCharCountInField = maxCharCountInField;
        }

        /// <summary>
        /// Gets the maximum character count in single field.
        /// </summary>
        public int MaxCharCountInField { get; } = 500;

        /// <summary>
        /// Gets or sets the length of content to leave in field, when trimming.
        /// </summary>
        public int LeaveOnTrim { get; set; } = 10;

        /// <inheritdoc/>
        public string ContentType => "application/json";

        /// <inheritdoc/>
        public string Modify(Stream content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            content.Seek(0, SeekOrigin.Begin);

            var sb = new StringBuilder();
            using var streamReader = new StreamReader(content);
            using var reader = new JsonTextReader(streamReader);
            try
            {
                using var stringWriter = new StringWriter(sb);
                using var writer = new JsonTextWriter(stringWriter);
                while (reader != null && reader.Read())
                {
                    if (reader.Value != null || reader.TokenType == JsonToken.Null)
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            if (reader.Value != null)
                            {
                                string? key = reader.Value?.ToString();
                                writer.WritePropertyName(key, true);
                            }
                        }
                        else
                        {
                            object? value = this.GetValue(reader.TokenType, reader?.Value);
                            writer.WriteValue(value);
                        }
                    }
                    else
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                            writer.WriteStartObject();

                        if (reader.TokenType == JsonToken.StartArray)
                            writer.WriteStartArray();

                        if (reader.TokenType == JsonToken.EndArray)
                            writer.WriteEndArray();

                        if (reader.TokenType == JsonToken.EndObject)
                            writer.WriteEndObject();
                    }
                }

                stringWriter.Flush();
                return sb.ToString();
            }
            catch (Exception)
            {
                // Ignore content if we could not read it.
                content.Seek(0, SeekOrigin.Begin);
                return streamReader.ReadToEnd();
            }
        }

        private object? GetValue(JsonToken tokenType, object? value)
        {
            switch (tokenType)
            {
                case JsonToken.Bytes:
                case JsonToken.String:
                    string? trim = value?.ToString();
                    if (trim?.Length > MaxCharCountInField)
                        trim = trim.Substring(0, LeaveOnTrim) + "...";

                    return trim;

                default:
                    return value;
            }
        }
    }
}