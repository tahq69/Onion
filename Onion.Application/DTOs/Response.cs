#nullable disable
using System.Collections.Generic;

namespace Onion.Application.DTOs
{
    /// <summary>
    /// Response value wrapper.
    /// </summary>
    /// <typeparam name="T">Type of the response data.</typeparam>
    public class Response<T>
    {
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
        public IDictionary<string, ICollection<string>> Errors { get; set; }

        /// <summary>
        /// Gets or sets the response data.
        /// </summary>
        public T Data { get; set; }
    }
}