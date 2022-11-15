using Quorum.Hackathon.RateLimit.Concurrency;

namespace API.Config
{
    public class RateLimiterAndConfig
    {
        public IConcurrencyLimiter Limiter { get; set; }    
        public RateConfigItem Configuration { get; set; }
    }
}
