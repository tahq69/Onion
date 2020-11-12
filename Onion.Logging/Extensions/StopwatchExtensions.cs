using Onion.Logging.Interfaces;

namespace Onion.Logging
{
    internal static class StopwatchExtensions
    {
        public static string Time(this IStopwatch stopwatch) =>
            stopwatch.Elapsed.ToString(@"hh\:mm\:ss\:fff");
    }
}