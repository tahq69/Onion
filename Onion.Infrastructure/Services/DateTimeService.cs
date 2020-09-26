using System;
using Onion.Application.Interfaces;

namespace Onion.Shared.Data
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}