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
        /// <param name="input">The source stream to read from.</param>
        /// <param name="output">The target stream to write.</param>
        void Modify(Stream input, Stream output);
    }
}