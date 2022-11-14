using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class LimiterLease : ILimiterLease
    {
        private bool DisposedValue;
        private RateLimitLease? LimitLease = null;

        public LimiterLease(RateLimitLease? rateLimitLease) {
            LimitLease = rateLimitLease;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    LimitLease?.Dispose();
                    LimitLease = null;
                }

                DisposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
