using Core;
using Core.Configs;
using Core.Extensions;
using Core.Services;

namespace NasaClientService.Extensions;

public static class ServicesExtensions
{
    public static void AddNasaServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        var sectionName = configuration.GetSection("NasaDataset");
        services
            .AddOptions<NasaDatasetConfig>()
            .Bind(sectionName);


        services.AddRedis(configuration);
        services.AddTransient<IRedisCacheService, RedisCacheService>();
        
        services.AddDbContext<AppDbContext>();
        
        AddHttpClient(services);
        
        services.AddTransient<INasaHttpClient, NasaHttpClient>();
        services.AddTransient<INasaBackgroundService, NasaBackgroundService>();


        services.AddHostedService<NasaDatasetWorker>();
    }

    private static void AddHttpClient(IServiceCollection services)
    {
        var client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestHeaders = 
            {
                { "Accept", "application/json" }
            }
        };

        services.AddSingleton(client);
    }
}