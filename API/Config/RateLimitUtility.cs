using Quorum.Hackathon.RateLimit.Concurrency;

namespace API.Config
{
    public static class RateLimitUtility
    {
        static LimiterFactory Factory = new LimiterFactory();

        public static IConcurrencyLimiter GetRateLimiter(RateConfig rateConfiguration)
        {
            IConcurrencyLimiter limiter = null;
            /*     Factory.CreateLimiter(new LimiterFactoryOptions()
            {


            }); */
            return limiter;
        }

    }
}
