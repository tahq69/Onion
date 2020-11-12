using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    public interface IBasicInfoLogger
    {
        void LogBasicInfo(ILogger logger, LogLevel level, IStopwatch stopwatch, HttpContext context);
    }
}