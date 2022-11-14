using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Hackathon.RateLimit.Concurrency
{
    public class LimiterFactoryOptions
    {
        public LimitFactoryCreateType? Type { get; set; }   
        public int? PermitLimit { get; set; }
        public int? QueueLimit { get; set; }
        public TimeSpan? ReplenishPeriod { get; set; }
        public int? TokensReplenishedPerPeriod { get; set; }
        public bool? AutoReplenish { get; set; }
        public TimeSpan? Window { get; set; }
        public int? SegmentsPerWindow { get; set; }
    }

    public enum LimitFactoryCreateType
    {
        RateConcurrent = 0,
        TokenRate = 1,
        WindowRate = 2
    }
}
