var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:"AllowAnyOrigin", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin());
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();