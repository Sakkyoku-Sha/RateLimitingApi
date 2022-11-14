using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class WindowRateLimiter : ConcurrencyLimiterBase
    {
        public WindowRateLimiter(int permitLimit, int queueLimit, TimeSpan window, int segmentsPerWindow, bool autoReplenish)
        {
            Limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions() { PermitLimit = permitLimit, QueueProcessingOrder = QueueProcessingOrder.OldestFirst, 
                QueueLimit = queueLimit, Window = window, SegmentsPerWindow = segmentsPerWindow, AutoReplenishment = autoReplenish });
        }
    }
}
