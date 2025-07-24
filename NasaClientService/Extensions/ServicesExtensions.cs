namespace NasaClientService.Extensions;

public static class ServicesExtensions
{
    public static void AddNasaServices(this IServiceCollection services, IConfiguration configuration)
    {
        var sectionName = configuration.GetSection("NasaDataset");
        
        services
            .AddOptions<NasaDatasetConfig>()
            .Bind(sectionName);
        
        AddHttpClient(services);

        services.AddSingleton<INasaDatasetClient, NasaDatasetClient>();


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