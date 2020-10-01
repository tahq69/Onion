using System;
using Newtonsoft.Json;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// JSON content builder contract.
    /// Allows to rewrite JSON keys and values by providing custom factory methods.
    /// </summary>
    public interface IJsonContentBuilder
    {
        /// <summary>
        /// Start new JSON content build.
        /// </summary>
        /// <param name="keyFactory">JSON element key factory.</param>
        /// <param name="valueFactory">JSON element value factory.</param>
        /// <returns>Initialized builder instance.</returns>
        IJsonContentBuilder Start(
            Func<object?, string?>? keyFactory = null,
            Func<JsonToken, object?, object?>? valueFactory = null);

        /// <summary>
        /// Add element to the JSON object.
        /// </summary>
        /// <param name="tokenType">The element type.</param>
        /// <param name="value">The element value.</param>
        /// <returns>Updated JSON content builder.</returns>
        IJsonContentBuilder AddElement(JsonToken tokenType, object? value);

        /// <summary>
        /// Build JSON string from added content.
        /// </summary>
        /// <returns>JSON formatted string.</returns>
        string Build();

        /// <summary>
        /// Build JSON string with custom factories from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">JSON content reader.</param>
        /// <param name="keyFactory">JSON element key factory.</param>
        /// <param name="valueFactory">JSON element value factory.</param>
        /// <returns>JSON formatted string.</returns>
        string Build(
            JsonTextReader reader,
            Func<object?, string?>? keyFactory = null,
            Func<JsonToken, object?, object?>? valueFactory = null);
    }
}