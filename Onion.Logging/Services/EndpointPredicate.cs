using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Onion.Logging.Interfaces;

namespace Onion.Logging.Services
{
    /// <summary>
    /// Filter requests from being logged by endpoint uri.
    /// </summary>
    public class EndpointPredicate : IHttpRequestPredicate
    {
        private readonly bool _exclude;
        private readonly Regex[] _expressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointPredicate"/> class.
        /// Include or exclude requests from logging based on endpoint.
        /// </summary>
        /// <param name="exclude">True to disable any request from logging excluding those of pattern set.</param>
        /// <param name="patterns">List of wildcard patterns to be logged if exclude set to true.</param>
        public EndpointPredicate(bool exclude, IEnumerable<string> patterns)
        {
            _exclude = exclude;
            _expressions = patterns
                .Select(i => "^" + Regex
                    .Escape(i)
                    .Replace(@"\*", ".*", StringComparison.InvariantCulture)
                    .Replace(@"\?", ".", StringComparison.InvariantCulture) + "$")
                .Select(i => new Regex(i, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                .ToArray();
        }

        /// <inheritdoc/>
        public bool Filter(HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            string url = req.Path;
            var matches = _expressions
                .Select(i => i.IsMatch(url))
                .Any(i => i);

            return (_exclude && !matches) || (!_exclude && matches);
        }
    }
}