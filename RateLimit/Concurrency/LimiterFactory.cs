using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class LimiterFactory : ILimiterFactory
    {
        public IConcurrencyLimiter CreateLimiter(LimiterFactoryOptions options)
        {
            IConcurrencyLimiter concurrencyLimiter = null;    
            if (options.Type.HasValue) { 
                switch (options.Type.Value)
                {
                    case LimitFactoryCreateType.RateConcurrent:
                        if (options.PermitLimit.HasValue && options.QueueLimit.HasValue)
                        {
                            concurrencyLimiter = CreateConcurrencyLimiter(options.PermitLimit.Value, options.QueueLimit.Value);
                        }
                        break;

                    case LimitFactoryCreateType.TokenRate:
                        {
                            var tokensReplenished = options.TokensReplenishedPerPeriod.HasValue ? options.TokensReplenishedPerPeriod.Value : 1;
                            var autoReplenish = options.AutoReplenish.HasValue ? options.AutoReplenish.Value : false;
                            if (options.PermitLimit.HasValue && options.QueueLimit.HasValue && options.ReplenishPeriod.HasValue)
                            {
                                concurrencyLimiter = CreateTokenLimiter(options.PermitLimit.Value, options.QueueLimit.Value, options.ReplenishPeriod.Value,
                                    tokensReplenished, autoReplenish);
                            }
                        }
                        break;

                    case LimitFactoryCreateType.WindowRate:
                        {
                            var segmentsPerWindow = options.SegmentsPerWindow.HasValue ? options.SegmentsPerWindow.Value : 1;
                            var autoReplenish = options.AutoReplenish.HasValue ? options.AutoReplenish.Value : false;
                            if (options.PermitLimit.HasValue && options.QueueLimit.HasValue && options.Window.HasValue)
                            {
                                concurrencyLimiter = CreateWindowRateLimiter(options.PermitLimit.Value, options.QueueLimit.Value, options.Window.Value,
                                    segmentsPerWindow, autoReplenish);
                            }
                        }
                        break;
                }
            }
            return concurrencyLimiter;
        }

        public IConcurrencyLimiter CreateConcurrencyLimiter(int permitLimit, int queueLimit)
        {
            return new RateConcurrencyLimiter(permitLimit, queueLimit);
        }

        public IConcurrencyLimiter CreateTokenLimiter(int permitLimit, int queueLimit, TimeSpan replenishPeriod, int tokensReplenishedPerPeriod, bool autoReplenish)
        {
            return new TokenLimiter(permitLimit, queueLimit, replenishPeriod, tokensReplenishedPerPeriod, autoReplenish);
        }

        public IConcurrencyLimiter CreateWindowRateLimiter(int permitLimit, int queueLimit, TimeSpan window, int segmentsPerWindow, bool autoReplenish)
        {
            return new WindowRateLimiter(permitLimit, queueLimit, window, segmentsPerWindow, autoReplenish);
        }

        public IConcurrencyLimiter CreatedPartitionedLimiter<TResource, TPartitionKey>(TResource resource, 
            Func<TResource, RateLimitPartition<TPartitionKey>> limiterFunc) where TPartitionKey : notnull
        {
            return new PartitionedLimiter<TResource, TPartitionKey>(resource, limiterFunc);
        }
    }
}
