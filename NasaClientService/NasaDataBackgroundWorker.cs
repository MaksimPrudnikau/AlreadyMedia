using Core.Configs;
using Core.Services;
using Microsoft.Extensions.Options;

namespace NasaClientService;

public class NasaDatasetWorker(
    IServiceProvider services,
    ILogger<NasaDatasetWorker> logger,
    IOptions<NasaDatasetConfig> options,
    INasaBackgroundService nasaService
) : BackgroundService
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
            await using var scope = services.CreateAsyncScope();

            var result = await nasaService.UpsertDatasetsAsync();

            logger.LogInformation("Successfully added {count} NASA images", result.added);
            logger.LogInformation("Successfully removed {count} NASA images", result.removed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during NASA data sync");
        }
    }

    private async Task RetryUpdate(CancellationToken stoppingToken)
    {
        var syncInterval = options.Value.SyncIntervalSeconds;
        await Task.Delay(TimeSpan.FromSeconds(syncInterval), stoppingToken);
    }
}