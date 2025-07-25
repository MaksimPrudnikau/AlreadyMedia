using NasaClientService.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddNasaServices(builder.Configuration);

var host = builder.Build();
host.Run();