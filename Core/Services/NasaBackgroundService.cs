using Core.Configs;
using Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaBackgroundService
{
    Task<SyncResult> UpsertDatasetsAsync(CancellationToken ct = default);
}

public sealed class NasaBackgroundService(
    AppDbContext db,
    INasaHttpClient nasaClient,
    INasaDatabaseSynchronizer synchronizer,
    IOptions<NasaDatasetConfig> options,
    ILogger<NasaBackgroundService> logger) : INasaBackgroundService
{
    private const int BatchSize = 900;
    
    public async Task<SyncResult> UpsertDatasetsAsync(CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var syncResult = new SyncResult();

        try
        {
            syncResult = await TryUpsertDatasetsAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (HttpRequestException e)
        {
            throw new ApplicationException(
                $"NASA data sync failed after {options.Value.MaxRetries} attempts. Last error: {e.Message}");
        }
        catch
        {
            await tx.RollbackAsync(ct);
        }

        return syncResult;
    }
    
    private async Task<SyncResult> TryUpsertDatasetsAsync(CancellationToken ct = default)
    {
        var batchedDatasets = await nasaClient.GetBatchedDatasetsAsync(BatchSize, ct);
        var syncResults = synchronizer.ApplyAsync(batchedDatasets, ct);
        
        var syncResult = new SyncResult();

        logger.LogInformation("Started batched sync with {BatchSize} batch size", BatchSize);
        await foreach (var batchResult in syncResults)
        {
            if (batchResult.IsEmpty)
            {
                continue;
            }
            
            syncResult += batchResult;
            logger.LogInformation("Successfully batched: +{Added}, -{Removed}, +-{Updated}", 
                batchResult.Added, batchResult.Removed, batchResult.Updated);
        }

        return syncResult;
    }
}