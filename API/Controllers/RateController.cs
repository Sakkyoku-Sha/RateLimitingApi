using API.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/Rate")]
    [ApiController]
    public class RateController : ControllerBase
    {
        RateConfig _rateConfig;

        public RateController(IOptions<RateConfig> rateOptions) { 
          _rateConfig= rateOptions.Value;
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
