using Core.Exceptions;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface INasaBackgroundService
{
    Task<SyncResult> UpsertDatasetsAsync(CancellationToken ct = default);
}

public sealed class NasaBackgroundService(
    INasaHttpClient nasaClient,
    INasaDatabaseSynchronizer synchronizer,
    ILogger<NasaBackgroundService> logger) : INasaBackgroundService
{
    public async Task<SyncResult> UpsertDatasetsAsync(CancellationToken ct = default)
    {
        var remoteDatasets = await nasaClient.GetDatasetAsync(ct);
        if (remoteDatasets is { Count: > 0 })
        {
            return await synchronizer.ApplyAsync(remoteDatasets, ct);
        }

        logger.LogInformation("NASA returned an empty list, nothing to sync.");
        return new(0, 0, 0);

    }
}