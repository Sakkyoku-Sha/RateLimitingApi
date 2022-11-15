using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class PartitionedLimiter<TResource, TPartitionKey> : IConcurrencyLimiter where TPartitionKey : notnull
    {
        TResource Resource { get; set; }
        PartitionedRateLimiter<TResource> Limiter { get; set; }

        public PartitionedLimiter(TResource resource, Func<TResource, RateLimitPartition<TPartitionKey>> limiterFunc) {
            Limiter = PartitionedRateLimiter.Create<TResource, TPartitionKey>(limiterFunc);
            Resource = resource;
        }

        public ILimiterLease AttemptAcquire()
        {
            var limiterLease = Limiter.AttemptAcquire(Resource);
            return new LimiterLease(limiterLease);
        }

        public RateLimiter GetRateLimiter() => throw new InvalidOperationException();

        public RateLimiterStatistics GetStatistics()
        {
            return Limiter.GetStatistics(Resource);
        }

        public async Task<ILimiterLease> WaitASync()
        {
            var limiterLease = await Limiter.AcquireAsync(Resource);
            return new LimiterLease(limiterLease);
        }
    }
}
