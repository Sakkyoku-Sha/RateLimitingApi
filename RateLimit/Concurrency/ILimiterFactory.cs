using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public interface ILimiterFactory
    {
        IConcurrencyLimiter CreateLimiter(LimiterFactoryOptions options);
        IConcurrencyLimiter CreateConcurrencyLimiter(int permitLimit, int queueLimit);
        IConcurrencyLimiter CreateTokenLimiter(int permitLimit, int queueLimit, TimeSpan replenishPeriod, int tokensReplenishedPerPeriod, bool autoReplenish);
        IConcurrencyLimiter CreateWindowRateLimiter(int permitLimit, int queueLimit, TimeSpan window, int segmentsPerWindow, bool autoReplenish);
        IConcurrencyLimiter CreatedPartitionedLimiter<TResource, TPartitionKey>(TResource resource, Func<TResource, RateLimitPartition<TPartitionKey>> limiterFunc) where TPartitionKey : notnull;
    }
}
