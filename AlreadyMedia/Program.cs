using System.Configuration;
using Core;
using Core.Extensions;
using Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddHttpClient<INasaHttpClient, NasaHttpClient>();

builder.Services.AddScoped<INasaDatabaseSynchronizer, NasaDatabaseSynchronizer>();

builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();
builder.Services.AddTransient<INasaCacheService, NasaCacheService>();
builder.Services.AddTransient<INasaService, NasaService>();
builder.Services.AddTransient<INasaBackgroundService, NasaBackgroundService>();

builder.Services.AddControllers();

const string frontendCorsPolicyName = "FrontendPolicy";
var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl");
if (frontendUrl is null)
{
    throw new ConfigurationErrorsException("Frontend Url is not defined");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicyName, policy =>
    {
        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseCors(frontendCorsPolicyName);

app.Run();