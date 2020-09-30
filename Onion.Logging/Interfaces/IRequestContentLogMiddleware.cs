using System.IO;

namespace Onion.Logging.Interfaces
{
    /// <summary>
    /// Logging request content middleware contract.
    /// </summary>
    public interface IRequestContentLogMiddleware
    {
        /// <summary>
        /// Gets the type of the content to parse.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Parses the specified request content to prepare it for logging.
        /// </summary>
        /// <param name="content">The source stream to read from.</param>
        /// <returns>
        /// <paramref name="content"/> converted to string and prepared for logging without large data.
        /// </returns>
        string Modify(Stream content);
    }
}