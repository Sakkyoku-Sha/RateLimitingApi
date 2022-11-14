using Quorum.Hackathon.RateLimit.Concurrency;
using System.Threading.RateLimiting;

namespace API.Config
{
    public static class RateLimitUtility
    {
        static LimiterFactory Factory = new LimiterFactory();

        public static IConcurrencyLimiter GetRateLimiter(RateConfig rateConfiguration)
        {
            IConcurrencyLimiter limiter = Factory.CreateLimiter(new LimiterFactoryOptions()
            {
                Type = rateConfiguration.Type,
                PermitLimit = rateConfiguration.PermitLimit,
                QueueLimit = rateConfiguration.QueueLimit,
                ReplenishPeriod = rateConfiguration.ReplenishPeriod,
                TokensReplenishedPerPeriod = rateConfiguration.TokensReplenishedPerPeriod,
                AutoReplenish = rateConfiguration.AutoReplenish,
                Window = rateConfiguration.Window,
                SegmentsPerWindow= rateConfiguration.SegmentsPerWindow
            });
            
            return limiter;
        }

        public static IConcurrencyLimiter GetPartitionLimiter(string strResource, string strKey, IConcurrencyLimiter baseLimiter)
        {
            return Factory.CreatedPartitionedLimiter<string, string>(strResource, resource =>
                RateLimitPartition.Get(strKey, key => baseLimiter.GetRateLimiter()));
        }

    }
}
