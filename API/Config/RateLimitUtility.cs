using Microsoft.AspNetCore.DataProtection.KeyManagement;
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

        public static RateLimiterAndConfig GetPartitionLimiter(RatePartitionConfigAndLimiter partitionConfig, string strKey, 
            IDictionary<string, RateLimiterAndConfig> limiterMap, IDictionary<string, RateLimiter> rateLimiters)
        {
            var configuration = limiterMap[partitionConfig.LimiterName].Configuration;
            /* if (partitionConfig.Limiter == null)
            {
                partitionConfig.Limiter = Factory.CreatedPartitionedLimiter<string, string>(partitionConfig.Resource, resource =>
                RateLimitPartition.Get(strKey,
                key =>
                {
                    string limiterName = GetPartitionLimiterName(partitionConfig.Resource, strKey);
                    if (rateLimiters.ContainsKey(limiterName))
                        return rateLimiters[limiterName];
                    else if (partitionConfig.SharePartitionLimiter == true)
                        return limiterMap[partitionConfig.LimiterName].Limiter.GetRateLimiter();
                    else
                    {
                        var limiter = GetRateLimiter(configuration);
                        rateLimiters[limiterName] = limiter.GetRateLimiter();
                        return rateLimiters[limiterName];
                    }
                }));
            } */

            return new RateLimiterAndConfig
            {
                Configuration = configuration,
                Limiter = Factory.CreatedPartitionedLimiter<string, string>(partitionConfig.Resource, resource =>
                    RateLimitPartition.Get(strKey,
                    key =>
                    {
                        string limiterName = GetPartitionLimiterName(partitionConfig.Resource, strKey);
                        if (rateLimiters.ContainsKey(limiterName))
                            return rateLimiters[limiterName];
                        else if (partitionConfig.SharePartitionLimiter == true)
                            return limiterMap[partitionConfig.LimiterName].Limiter.GetRateLimiter();
                        else
                        {
                            var limiter = GetRateLimiter(configuration);
                            rateLimiters[limiterName] = limiter.GetRateLimiter();
                            return rateLimiters[limiterName];
                        }
                    }))
            };
        }

        public static string GetPartitionLimiterName(string strResource, string strKey) => $"{strResource}.{strKey}";
    }
}
