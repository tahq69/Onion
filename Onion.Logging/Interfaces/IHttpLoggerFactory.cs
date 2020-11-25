using Microsoft.Extensions.Logging;

namespace Onion.Logging
{
    public interface IHttpLoggerFactory
    {
        IHttpLogger Create(ILogger logger);
    }
}