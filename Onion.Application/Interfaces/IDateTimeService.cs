﻿using System;

namespace Onion.Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}