using System.Runtime.CompilerServices;
using Core.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaDatabaseSynchronizer
{
    IAsyncEnumerable<SyncResult> ApplyAsync(IAsyncEnumerable<ICollection<NasaDataset>> batchedDatasets, CancellationToken ct = default);
}

public sealed class NasaDatabaseSynchronizer(AppDbContext db)
    : INasaDatabaseSynchronizer
{
    private readonly static BulkConfig BulkConfig = new()
    {
        EnableStreaming = true,
        UpdateByProperties = [nameof(NasaDataset.Id)],
        ConflictOption = ConflictOption.Replace,
        CalculateStats = true,
    };

    public async IAsyncEnumerable<SyncResult> ApplyAsync(
        IAsyncEnumerable<ICollection<NasaDataset>> batchedDatasets,
        [EnumeratorCancellation]CancellationToken ct = default)
    {
        var remoteIds = new HashSet<int>();
        await foreach (var batch in batchedDatasets.WithCancellation(ct))
        {
            (int added, int updated) = await UpsertAsync(batch, ct);
            yield return new(added, 0, updated);
            remoteIds.UnionWith(batch.Select(x => x.Id).ToArray());
        }

        var removed = await db.NasaDbSet
            .Where(x => !remoteIds.Contains(x.Id))
            .ExecuteDeleteAsync(ct);

        yield return new(0, removed);
    }

    private async Task<(int, int)> UpsertAsync(ICollection<NasaDataset> datasets, CancellationToken ct)
    {
        if (datasets.Count == 0)
        {
            return (0, 0);
        }

        await db.BulkInsertOrUpdateAsync(datasets, BulkConfig, cancellationToken: ct);
        var inserted = BulkConfig.StatsInfo?.StatsNumberInserted ?? 0;
        var updated = BulkConfig.StatsInfo?.StatsNumberUpdated ?? 0;

        return (inserted, updated);
    }
}