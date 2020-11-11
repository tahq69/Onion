using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    public interface IContextLogger
    {
        IContextLogger FromContext(HttpContext context);

        Task LogRequest(LogLevel level);

        Task LogResponse(LogLevel level, IStopwatch stopwatch);

        void LogInfo(IStopwatch stopwatch);

        void LogError(Exception exception, IStopwatch stopwatch);

        LogLevel GetLogLevel();
    }
}