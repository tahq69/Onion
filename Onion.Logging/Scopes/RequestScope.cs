﻿using System.Collections.Generic;

namespace Onion.Logging.Scopes
{
    /// <summary>
    /// HTTP request logging scope.
    /// </summary>
    internal class RequestScope : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScope"/> class.
        /// </summary>
        /// <param name="endpoint">The request endpoint address.</param>
        /// <param name="methhod">HTTP request method.</param>
        public RequestScope(string endpoint, string methhod)
            : base(3)
        {
            Add("EventName", "HttpRequest");
            Add("Endpoint", endpoint);
            Add("HttpMethod", methhod);
        }
    }
}