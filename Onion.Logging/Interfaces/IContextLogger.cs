using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// HTTP context logger contract.
    /// </summary>
    public interface IContextLogger
    {
        /// <summary>
        /// Gets current logger level.
        /// </summary>
        LogLevel LogLevel { get; }

        /// <summary>
        /// Write request log.
        /// </summary>
        /// <param name="level">Current logging level for write.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogRequest(LogLevel level);

        /// <summary>
        /// Write response log.
        /// </summary>
        /// <param name="level">Current logging level for write.</param>
        /// <param name="stopwatch">Request execution performance watch state.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LogResponse(LogLevel level, IStopwatch stopwatch);

        /// <summary>
        /// Write request basic information log.
        /// </summary>
        /// <param name="level">Current logging level for write.</param>
        /// <param name="stopwatch">Request execution performance watch state.</param>
        void LogInfo(LogLevel level, IStopwatch stopwatch);

        /// <summary>
        /// Write request execution error log.
        /// </summary>
        /// <param name="exception">Execution error instance.</param>
        /// <param name="stopwatch">Request execution performance watch state.</param>
        void LogError(Exception exception, IStopwatch? stopwatch);
    }
}