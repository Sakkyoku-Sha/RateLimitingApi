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

        RateConfig? _rateConfig;
        IDictionary<string, RateLimiterAndConfig> _rateLimiters = null;
        IDictionary<string, RatePartitionConfig> _partitionConfigMap = null;

        public RateController(IOptions<RateConfig> rateOptions)
        {
            _rateConfig = rateOptions.Value;
            _rateLimiters = RateLimitUtility.GetRateLimiters(_rateConfig!);
            if (_rateConfig.Partitions != null)
            {
                _partitionConfigMap = _rateConfig.Partitions.ToDictionary(x => x.Name, x => x);
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
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("AddPartitionLimiter")]
        public bool AddPartitionLimiter(string Partition, string Key)
        {
            if (_partitionConfigMap.ContainsKey(Partition) == false)
                return false;

            var partitionConfig = _partitionConfigMap[Partition];
            var limiterName = $"{partitionConfig.Resource}.{Key}";
            if (_rateLimiters.ContainsKey(limiterName))
                return false;

            var rateLimiterAndConfig = RateLimitUtility.GetPartitionLimiter(partitionConfig, Key,
                _rateLimiters);
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
                    $"Successful Leases: {statistics.TotalSuccessfulLeases}{Environment.NewLine}Failed Leases:{statistics.TotalFailedLeases}{Environment.NewLine}";
            }
            return "Can't find limiter";
        } 

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("TryRateLimit")]
        public async Task<bool> TryRateLimit(string limiterName)
        {
            bool acquiredLease = false;
            if (_rateLimiters.ContainsKey(limiterName))
            {
                var limiterAndConfig = _rateLimiters[limiterName];
                using (var lease = await limiterAndConfig.Limiter.WaitASync())
                {
                    acquiredLease = lease.IsAcquired;
                }
            }
            return acquiredLease;
        }

        // POST api/<RateController>


        // DELETE api/<RateController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
