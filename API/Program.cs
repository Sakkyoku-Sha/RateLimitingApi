using API.Config;
using Microsoft.AspNetCore.RateLimiting;
using Quorum.Hackathon.RateLimit.Concurrency;
using System.Diagnostics;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.development.json");

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

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();