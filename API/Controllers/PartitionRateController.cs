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
    [Microsoft.AspNetCore.Mvc.Route("api/PartitionRate")]
    [ApiController]
    public class PartitionRateController : ControllerBase
    {
        static RateConfig? _rateConfig = null;
        static int _counter = 1;

        public PartitionRateController(IOptions<RateConfig> rateOptions)
        {
            if (_rateConfig == null)
            {
                _rateConfig = rateOptions.Value;
            }
        }

        #region Properties


        #endregion


        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("TryRateLimit")]
        [EnableRateLimiting("post")]
        public string TryRateLimit([FromHeader]string Role)
        {
            return $"Counter: {_counter++}";
        }

        // POST api/<RateController>


        // DELETE api/<RateController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
