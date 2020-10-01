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
        private readonly IJsonContentBuilder _jsonBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LongJsonContentMiddleware"/> class.
        /// </summary>
        /// <param name="jsonBuilder">JSON content builder.</param>
        public LongJsonContentMiddleware(IJsonContentBuilder jsonBuilder)
        {
            _jsonBuilder = jsonBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LongJsonContentMiddleware"/> class.
        /// </summary>
        /// <param name="jsonBuilder">JSON content builder.</param>
        /// <param name="maxCharCountInField">The maximum character count in field.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="maxCharCountInField"/> is less than 1.
        /// </exception>
        public LongJsonContentMiddleware(IJsonContentBuilder jsonBuilder, int maxCharCountInField)
            : this(jsonBuilder)
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

            using var streamReader = new StreamReader(content);
            using var reader = new JsonTextReader(streamReader);
            try
            {
                return _jsonBuilder.Build(reader, GetKey, GetValue);
            }
            catch (Exception)
            {
                // Ignore content if we could not read it.
                content.Seek(0, SeekOrigin.Begin);
                return streamReader.ReadToEnd();
            }
        }

        private string? GetKey(object? key) =>
            key?.ToString();

        /// <summary>
        /// We will trim Bytes|String value the MaxCharCountInField length.
        /// </summary>
        /// <param name="tokenType">Token value type.</param>
        /// <param name="value">The actual value.</param>
        /// <returns>Updated or original value.</returns>
        private object? GetValue(JsonToken tokenType, object? value)
        {
            if (value is null)
                return null;

            switch (tokenType)
            {
                case JsonToken.Bytes:
                case JsonToken.String:
                    string? val = value.ToString();
                    if (val?.Length > MaxCharCountInField)
                        val = $"{val.Substring(0, LeaveOnTrim)}...";

                    return val;

                default:
                    return value;
            }
        }
    }
}