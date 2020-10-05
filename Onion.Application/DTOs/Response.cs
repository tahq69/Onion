#nullable disable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Onion.Shared.Extensions;

namespace Onion.Application.DTOs
{
    /// <summary>
    /// Response value wrapper.
    /// </summary>
    /// <typeparam name="T">Type of the response data.</typeparam>
    public class Response<T>
    {
        private IDictionary<string, ICollection<string>> _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class.
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/> class with
        /// successful status.
        /// </summary>
        /// <param name="data">The data for response.</param>
        /// <param name="message">The response message.</param>
        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}"/>
        /// class with unsuccessful status.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="errors">The dictionary with errors.</param>
        public Response(string message, IDictionary<string, ICollection<string>> errors = null)
        {
            Succeeded = false;
            Message = message;
            Errors = errors;
        }

        /// <summary>
        /// Gets or sets a value indicating whether response is successful.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets response errors dictionary.
        /// </summary>
        public IDictionary<string, ICollection<string>> Errors
        {
            get => _errors;
            set
            {
                _errors = value.ToDictionary(
                    e => e.Key.ToLowerFirstChar(),
                    e => e.Value);
            }
        }

        /// <summary>
        /// Gets or sets the response data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Create error response with message and single error field.
        /// </summary>
        /// <param name="propName">Error field name.</param>
        /// <param name="message">Error message.</param>
        /// <typeparam name="TData">Type of the record data.</typeparam>
        /// <returns>Response message with filled error field.</returns>
        public static Response<TData> Error<TData>(string propName, string message)
            where TData : T =>
            Response<TData>.Error(default(TData), propName, message);

        /// <summary>
        /// Create error response with message and single error field.
        /// </summary>
        /// <param name="data">Response data field value.</param>
        /// <param name="propName">Error field name.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Response message with filled error field.</returns>
        public static Response<T> Error(T data, string propName, string message) =>
            new Response<T>
            {
                Message = message,
                Succeeded = false,
                Data = data,
                Errors = new Dictionary<string, ICollection<string>>
                {
                    { propName, new[] { message } },
                },
            };
    }
}