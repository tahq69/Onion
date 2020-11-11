using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Onion.Logging.Interfaces
{
    public interface IResponseLogger
    {
        Task LogResponse(ILogger logger, LogLevel level, IStopwatch stopwatch, HttpContext context);
    }
}