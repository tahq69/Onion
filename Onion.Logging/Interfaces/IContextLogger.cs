using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    public interface IContextLogger
    {
        LogLevel LogLevel { get; }

        Task LogRequest(LogLevel level);

        Task LogResponse(LogLevel level, IStopwatch stopwatch);

        void LogInfo(LogLevel level, IStopwatch stopwatch);

        void LogError(Exception exception, IStopwatch stopwatch);
    }
}