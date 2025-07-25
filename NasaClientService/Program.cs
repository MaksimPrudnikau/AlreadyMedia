using Core;
using NasaClientService.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddNasaServices(builder.Configuration);

var host = builder.Build();
host.Run();