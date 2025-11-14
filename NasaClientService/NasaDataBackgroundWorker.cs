using Core.Configs;
using Core.Services;
using Microsoft.Extensions.Options;

namespace NasaClientService;

public sealed class NasaDatasetWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<NasaDatasetWorker> logger,
    IOptions<NasaDatasetConfig> options
) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NASA Dataset Worker started.");

        var interval = TimeSpan.FromSeconds(options.Value.SyncIntervalSeconds);
        logger.LogInformation("Sync interval set to {Interval}s", interval.TotalSeconds);

        using var timer = new PeriodicTimer(interval);
        do
        {
            await ExecuteUpsertAsync(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));

        logger.LogInformation("NASA Dataset Worker stopped (timer ended).");
    }

    private async Task ExecuteUpsertAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var nasaService = scope.ServiceProvider.GetRequiredService<INasaBackgroundService>();

        logger.LogInformation("Starting NASA data sync at {Time}", DateTimeOffset.Now);

        var result = await nasaService.UpsertDatasetsAsync(ct);

        if (result.IsEmpty)
        {
            logger.LogInformation("Nothing to sync");
            return;
        }

        logger.LogInformation(
            "NASA sync completed successfully: +{Added}, -{Removed}, +-{Updated}",
            result.Added,
            result.Removed,
            result.Updated);
    }
}