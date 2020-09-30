using System;

namespace Onion.Application.Interfaces
{
    /// <summary>
    /// Datetime service contract.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets current UTC date and time.
        /// </summary>
        DateTime UtcNow { get; }
    }
}