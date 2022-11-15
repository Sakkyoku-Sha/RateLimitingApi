using Quorum.Hackathon.RateLimit.Concurrency;
using System.Threading.RateLimiting;

namespace API.Config
{
    public static class RateLimitUtility
    {
        static LimiterFactory Factory = new LimiterFactory();

        public static IDictionary<string, RateLimiterAndConfig> GetRateLimiters(RateConfig rateConfiguration)
        {
            IDictionary<string, RateLimiterAndConfig> limiterMap = new Dictionary<string, RateLimiterAndConfig>();
            foreach (var rateConfigItem in rateConfiguration.Limiters)
            {
                var limiter = GetRateLimiter(rateConfigItem);
                limiterMap[rateConfigItem.Name] = new RateLimiterAndConfig { Limiter = limiter, Configuration = rateConfigItem };
            }

            return limiterMap;
        }

        static IConcurrencyLimiter GetRateLimiter(RateConfigItem rateConfigItem) => Factory.CreateLimiter(new LimiterFactoryOptions()
        {
            Type = rateConfigItem.Type,
            PermitLimit = rateConfigItem.PermitLimit,
            QueueLimit = rateConfigItem.QueueLimit,
            ReplenishPeriod = rateConfigItem.ReplenishPeriod,
            TokensReplenishedPerPeriod = rateConfigItem.TokensReplenishedPerPeriod,
            AutoReplenish = rateConfigItem.AutoReplenish,
            Window = rateConfigItem.Window,
            SegmentsPerWindow = rateConfigItem.SegmentsPerWindow
        });

        public static RateLimiterAndConfig GetPartitionLimiter(RatePartitionConfig partitionConfig, string strKey, IDictionary<string, RateLimiterAndConfig> rateLimiters)
        {
            var configuration = rateLimiters[partitionConfig.LimiterName].Configuration;
            return new RateLimiterAndConfig
            {
                Configuration = configuration,
                Limiter = Factory.CreatedPartitionedLimiter<string, string>(partitionConfig.Resource, resource =>
                RateLimitPartition.Get(strKey,
                key =>
                {
                    if (partitionConfig.SharePartitionLimiter == true)
                        return rateLimiters[partitionConfig.LimiterName].Limiter.GetRateLimiter();
                    else
                    {

                        var limiter = GetRateLimiter(configuration);
                        return limiter.GetRateLimiter();
                    }
                }))
            };
        }
    }
}
