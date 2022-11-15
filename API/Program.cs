using API.Config;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Primitives;
using Quorum.Hackathon.RateLimit.Concurrency;
using System.Diagnostics;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rateConfig = builder.Configuration.GetSection("RateConfig");
builder.Services.Configure<RateConfig>(rateConfig);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:"AllowAnyOrigin", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin());
});

var tokenPolicy = "token";

builder.Services.AddRateLimiter(_ => _
    .AddTokenBucketLimiter(policyName: tokenPolicy, options =>
    {
        options.QueueLimit = 1;
        options.TokenLimit = 5;
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        options.AutoReplenishment = true;
        options.TokensPerPeriod = 1;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
    }));


var app = builder.Build();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var rateLimitPartitionMap = new Dictionary<string, RateLimitPartition<string>>();
app.UseRateLimiter(new RateLimiterOptions()
       .AddPolicy<string>(policyName: "post", partitioner: httpContext =>
       {
           var headers = httpContext.Request.Headers;
           var role = httpContext.Request.Headers["Role"].ToString().ToLower();
           if (rateLimitPartitionMap.TryGetValue(role, out var rateLimitPartition))
               return rateLimitPartition;

           switch (role)
           {
               case "admin":
                   rateLimitPartition = RateLimitPartition.GetNoLimiter("nolimit");
                   break;

               case "superuser":
                   rateLimitPartition = RateLimitPartition.GetConcurrencyLimiter("concurrent", key =>
                        new ConcurrencyLimiterOptions()
                        {
                            PermitLimit = 5,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 1,
                        }
                        );
                   break;
               case "user":
               default:
                   rateLimitPartition = RateLimitPartition.GetTokenBucketLimiter("token", key =>
                       new TokenBucketRateLimiterOptions()
                       {
                           TokenLimit = 5,
                           QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                           QueueLimit = 1,
                           ReplenishmentPeriod = TimeSpan.FromSeconds(30),
                           TokensPerPeriod = 1
                       });
                   break;
           }
           rateLimitPartitionMap[role] = rateLimitPartition;
           return rateLimitPartition;
       }
   ));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();