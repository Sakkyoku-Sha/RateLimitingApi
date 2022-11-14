using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class TokenLimiter : ConcurrencyLimiterBase
    {
        public TokenLimiter(int tokenLimit, int queueLimit, TimeSpan replenishPeriod, int tokensReplenishedPerPeriod, bool autoReplenish)
        {
            Limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions() { TokenLimit = tokenLimit, QueueProcessingOrder = QueueProcessingOrder.OldestFirst, 
                QueueLimit = queueLimit, ReplenishmentPeriod = replenishPeriod, TokensPerPeriod = tokensReplenishedPerPeriod, AutoReplenishment = autoReplenish });
        }

        public override async Task<ILimiterLease> WaitASync()
        {
            var limiterLease = await Limiter.AcquireAsync();
            return new LimiterLease(limiterLease);
        }
    }
}
