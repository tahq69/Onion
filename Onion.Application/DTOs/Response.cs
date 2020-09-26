using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Onion.Application.DTOs
{
    /// <summary>
    /// Response value wrapper.
    /// </summary>
    /// <typeparam name="T">Type of the response data.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Onion.Application.DTOs.Response{T}"/> class.
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Onion.Application.DTOs.Response{T}(T, string)"/> class with
        /// successful status.
        /// </summary>
        /// <param name="data">The data for response.</param>
        /// <param name="message">The response message.</param>
        public Response([AllowNull] T data, [AllowNull] string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Onion.Application.DTOs.Response{T}(string, IDictionary{string, string})"/>
        /// class with unsuccessful status.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="errors">The dictionary with errors.</param>
        public Response(string message, [AllowNull] IDictionary<string, string> errors = null)
        {
            Succeeded = false;
            Message = message;
            Errors = errors;
        }

        /// <summary>
        /// Value indicating whenever response is successful.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// The response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response errors dictionary.
        /// </summary>
        public IDictionary<string, string> Errors { get; set; }

        /// <summary>
        /// The response data.
        /// </summary>
        public T Data { get; set; }
    }
}