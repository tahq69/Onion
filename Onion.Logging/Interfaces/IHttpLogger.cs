using System;
using System.Threading.Tasks;

namespace Onion.Logging
{
    /// <summary>
    /// HTTP request/response logger contract.
    /// </summary>
    public interface IHttpLogger
    {
        /// <summary>
        /// Write request log message.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogRequest(RequestDetails request);

        /// <summary>
        /// Write response log message.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <param name="response">The response details.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogResponse(RequestDetails request, ResponseDetails response);

        /// <summary>
        /// Write request and response basic information log message.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <param name="response">The response details.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogInfo(RequestDetails request, ResponseDetails response);

        /// <summary>
        /// Write request or response error log message.
        /// </summary>
        /// <param name="exception">Request or response exception.</param>
        /// <param name="request">The request details.</param>
        /// <param name="response">The response details.</param>
        void LogError(Exception exception, RequestDetails? request, ResponseDetails? response);
    }
}