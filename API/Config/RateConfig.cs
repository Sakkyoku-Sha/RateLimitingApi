using Quorum.Hackathon.RateLimit.Concurrency;

namespace API.Config
{
    public class RateConfig
    {
        public LimitFactoryCreateType Type { get; set; }
        public int PermitLimit { get; set; }
        public int? QueueLimit { get; set; }
        public TimeSpan? ReplenishPeriod { get; set; }
        public int? TokensReplenishedPerPeriod { get; set; }
        public bool? AutoReplenish { get; set; }
        public TimeSpan? Window { get; set; }
        public int? SegmentsPerWindow { get; set; }
        public RatePartitionConfig[]? PartitionResources { get; set; }   
    }

    public class RatePartitionConfig
    {
        public string Resource { get; set; }
        public string Key { get; set; }
    }
}
