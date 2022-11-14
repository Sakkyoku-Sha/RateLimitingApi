using API.Config;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGet("/", () => "Hello World!");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();