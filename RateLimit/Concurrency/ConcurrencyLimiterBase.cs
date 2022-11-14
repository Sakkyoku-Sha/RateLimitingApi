using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public abstract class ConcurrencyLimiterBase: IConcurrencyLimiter
    {
        protected RateLimiter Limiter { get; set; }

        public RateLimiter GetRateLimiter() => Limiter;
        public RateLimiterStatistics GetStatistics()
        {
            return Limiter.GetStatistics();
        }

        public async virtual Task<ILimiterLease> WaitASync()
        {
            throw new NotImplementedException();
        }
    }
}
