using Core.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaDatabaseSynchronizer
{
    Task<SyncResult> ApplyAsync(ICollection<NasaDataset> datasets, CancellationToken ct = default);
}

public sealed class NasaDatabaseSynchronizer(AppDbContext db)
    : INasaDatabaseSynchronizer
{
    private const int BatchSize = 900;

    private readonly static BulkConfig BulkConfig = new()
    {
        BatchSize = BatchSize,
        EnableStreaming = true,
        UpdateByProperties = [nameof(NasaDataset.Id)],
        ConflictOption = ConflictOption.Replace,
        CalculateStats = true,
    };
    
    public Task<SyncResult> ApplyAsync(ICollection<NasaDataset> datasets, CancellationToken ct = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(() => ApplyWithTransaction(datasets, ct));
    }

    private async Task<SyncResult> ApplyWithTransaction(ICollection<NasaDataset> datasets, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        try
        {
            int removed = await DeleteAsync(datasets, ct);
            (int added, int updated) = await UpsertAsync(datasets, ct);

            await tx.CommitAsync(ct);

            return new (added, removed, updated);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private Task<int> DeleteAsync(ICollection<NasaDataset> datasets, CancellationToken ct)
    {
        if (datasets.Count == 0)
        {
            return Task.FromResult(0);
        }

        var remoteIds = datasets.Select(ds => ds.Id).ToHashSet();
        
        return db.NasaDbSet
            .Where(ds => !remoteIds.Contains(ds.Id))
            .ExecuteDeleteAsync(ct);
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