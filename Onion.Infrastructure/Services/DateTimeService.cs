using System;
using Onion.Application.Interfaces;

namespace Onion.Infrastructure.Services
{
    /// <summary>
    /// Date time service implementation.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;
    }
}