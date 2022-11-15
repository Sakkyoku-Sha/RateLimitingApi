using API.Config;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quorum.Hackathon.RateLimit.Concurrency;
using System.Threading.RateLimiting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/Rate")]
    [ApiController]
    public class RateController : ControllerBase
    {
        const string NON_PARTITION_RESOURCE = "";

        static RateConfig? _rateConfig = null;
        static IDictionary<string, RateLimiterAndConfig> _rateLimiters = null;
        static IDictionary<string, RatePartitionConfigAndLimiter> _partitionMap = null;
        static IDictionary<string, RateLimiter> _partitionRateLimiters = new Dictionary<string, RateLimiter>();

        public RateController(IOptions<RateConfig> rateOptions)
        {
            if (_rateConfig == null)
            {
                _rateConfig = rateOptions.Value;
                _rateLimiters = RateLimitUtility.GetRateLimiters(_rateConfig!);
                if (_rateConfig.Partitions != null)
                {
                    _partitionMap = _rateConfig.Partitions.ToDictionary(x => x.Name, x => new RatePartitionConfigAndLimiter(x));
                }
            }
        }

        #region Properties


        #endregion


        // GET: api/<RateController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<RateController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("AddPartitionLimiter")]
        public bool AddPartitionLimiter(string Partition, string Key)
        {
            if (_partitionMap.ContainsKey(Partition) == false)
                return false;

            var partitionConfig = _partitionMap[Partition];
            var limiterName = RateLimitUtility.GetPartitionLimiterName(partitionConfig.Resource, Key);
            if (_rateLimiters.ContainsKey(limiterName))
                return false;

            var rateLimiterAndConfig = RateLimitUtility.GetPartitionLimiter(partitionConfig, Key,
                _rateLimiters, _partitionRateLimiters);
            _rateLimiters[limiterName] = rateLimiterAndConfig;
            return true;
        } 

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("GetStatistics")]
        public string GetStatistics(string limiterName)
        {
            if(_rateLimiters.ContainsKey(limiterName))
            {
                var limiterAndConfig = _rateLimiters[limiterName];
                var statistics = limiterAndConfig.Limiter.GetStatistics();
                return $"Available Permits: {statistics.CurrentAvailablePermits}{Environment.NewLine}Queue Count: {statistics.CurrentQueuedCount}{Environment.NewLine}" +
                    $"Successful Leases: {statistics.TotalSuccessfulLeases}{Environment.NewLine}Failed Leases: {statistics.TotalFailedLeases}{Environment.NewLine}";
            }
            return "Can't find limiter";
        } 

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("TryRateLimit")]
        public bool TryRateLimit(string limiterName)
        {
            bool acquiredLease = false;
            if (_rateLimiters.ContainsKey(limiterName))
            {
                var limiterAndConfig = _rateLimiters[limiterName];
                var lease = limiterAndConfig.Limiter.AttemptAcquire();
                acquiredLease = lease.IsAcquired;
            }
            return acquiredLease;
        }

        // POST api/<RateController>


        // DELETE api/<RateController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
