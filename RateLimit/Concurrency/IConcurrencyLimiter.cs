﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public interface IConcurrencyLimiter
    {
        Task<ILimiterLease> WaitASync();
    }
}
