using API.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Quorum.Hackathon.RateLimit.Concurrency;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/Rate")]
    [ApiController]
    public class RateController : ControllerBase
    {
        const string NON_PARTITION_RESOURCE = "";

        RateConfig? _rateConfig;
        IConcurrencyLimiter _baseLimiter;
        Dictionary<string, IConcurrencyLimiter> _rateLimiters = new Dictionary<string, IConcurrencyLimiter>();

        public RateController(IOptions<RateConfig> rateOptions)
        {
            _rateConfig = rateOptions.Value;
            _baseLimiter = RateLimitUtility.GetRateLimiter(_rateConfig);
            if (IsPartitionLimiter)
            {

                foreach (var partitionResource in _rateConfig.PartitionResources!)
                {
                    _rateLimiters[partitionResource.Resource] = RateLimitUtility.GetPartitionLimiter(
                        partitionResource.Resource, partitionResource.Key, _baseLimiter);
                }
            }
        }

        #region Properties

        bool IsPartitionLimiter => _rateConfig?.PartitionResources != null;

        #endregion

        IConcurrencyLimiter? GetLimiter(string strResource)
        {
            return _rateLimiters.ContainsKey(strResource) ? _rateLimiters[strResource] : null;
        }

        // GET: api/<RateController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RateController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RateController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RateController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RateController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
