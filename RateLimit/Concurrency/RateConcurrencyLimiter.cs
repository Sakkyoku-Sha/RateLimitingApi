using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class RateConcurrencyLimiter : ConcurrencyLimiterBase
    {
        public RateConcurrencyLimiter(int permitLimit, int queueLimit) { 
            Limiter = new ConcurrencyLimiter(new ConcurrencyLimiterOptions() { PermitLimit = permitLimit, QueueProcessingOrder = QueueProcessingOrder.OldestFirst, QueueLimit = queueLimit });
        }
    }
}
