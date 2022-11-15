using Quorum.Hackathon.RateLimit.Concurrency;

namespace API.Config
{
    public class RateConfig
    {
        public RateConfigItem[]? Limiters { get; set; }
        public RatePartitionConfig[]? Partitions { get; set; }
    }

    public class RateConfigItem
    { 
        public string Name { get; set; }
        public LimitFactoryCreateType Type { get; set; }
        public int PermitLimit { get; set; }
        public int? QueueLimit { get; set; }
        public TimeSpan? ReplenishPeriod { get; set; }
        public int? TokensReplenishedPerPeriod { get; set; }
        public bool? AutoReplenish { get; set; }
        public TimeSpan? Window { get; set; }
        public int? SegmentsPerWindow { get; set; }     
    }

    public class RatePartitionConfig
    {
        public string Name { get; set; }
        public string LimiterName { get; set; }
        public bool SharePartitionLimiter { get; set; }
        public string Resource { get; set; }
    }

    public class RatePartitionConfigAndLimiter : RatePartitionConfig
    {
        public RatePartitionConfigAndLimiter(RatePartitionConfig config)
        {
            Name = config.Name;
            LimiterName= config.LimiterName;
            SharePartitionLimiter= config.SharePartitionLimiter;    
            Resource = config.Resource;
        }

        public IConcurrencyLimiter? Limiter { get; set; }
    }
}
