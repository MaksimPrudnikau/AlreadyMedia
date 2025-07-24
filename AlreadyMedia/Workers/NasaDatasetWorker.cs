using AlreadyMedia.Configs;
using AlreadyMedia.Services;
using Core;
using Microsoft.Extensions.Options;

namespace AlreadyMedia.Workers;

public class NasaDatasetWorker(IServiceProvider services, ILogger<NasaDatasetWorker> logger, IOptions<NasaDatasetConfig> options) : BackgroundService
{
    
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateDataset();
            await RetryUpdate(stoppingToken);
        }
    }

    private async Task UpdateDataset()
    {
        logger.LogInformation("Starting NASA data sync at: {time}", DateTimeOffset.Now);

        try
        {
            var dataset = await GetDataset();

            // TODO: Save to local DB

            logger.LogInformation("Successfully synced {count} NASA images", dataset.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during NASA data sync");
        }
    }
    
    private async Task<ICollection<NasaDataset>> GetDataset()
    {
        using var scope = services.CreateScope();
        var nasaClient = scope.ServiceProvider.GetRequiredService<INasaDatasetClient>();
        return await nasaClient.GetDatasetAsync();
    }
    
    private async Task RetryUpdate(CancellationToken stoppingToken)
    {
        var syncInterval = options.Value.SyncIntervalSeconds; 
        await Task.Delay(TimeSpan.FromSeconds(syncInterval), stoppingToken);
    }
}