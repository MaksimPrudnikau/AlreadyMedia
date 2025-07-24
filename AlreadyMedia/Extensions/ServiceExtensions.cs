using AlreadyMedia.Services;
using AlreadyMedia.Workers;

namespace AlreadyMedia.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddNasaServices(this IServiceCollection services)
    {
        services.AddHttpClient<INasaDatasetClient, NasaDatasetClient>(client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHostedService<NasaDatasetWorker>();

        return services;
    }
}