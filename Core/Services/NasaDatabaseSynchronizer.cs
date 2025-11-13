using Core.Exceptions;
using Core.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public interface INasaDatabaseSynchronizer
{
    Task<SyncResult> ApplyAsync(ICollection<NasaDataset> datasets, CancellationToken ct = default);
}

public sealed class NasaDatabaseSynchronizer(AppDbContext db, ILogger<NasaDatabaseSynchronizer> log)
    : INasaDatabaseSynchronizer
{
    private const int BatchSize = 900;
    private readonly static BulkConfig BulkConfig = new() { BatchSize = BatchSize, EnableStreaming = true };
    
    public Task<SyncResult> ApplyAsync(ICollection<NasaDataset> datasets, CancellationToken ct = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(() => ApplyWithTransaction(datasets, ct));
    }

    private async Task<SyncResult> ApplyWithTransaction(ICollection<NasaDataset> datasets, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
            
        try
        {
            int removed = await DeleteAsync(datasets, ct);
            int added = await InsertAsync(datasets, ct);

            await tx.CommitAsync(ct);

            return new(added, removed);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);

            log.LogError(ex, "Database sync failed - transaction rolled back.");
            throw new NasaSyncException("Database synchronization failed", ex);
        }
    }

    private Task<int> DeleteAsync(ICollection<NasaDataset> fetchedDatasets, CancellationToken ct = default)
    {
        if (fetchedDatasets.Count == 0)
        {
            return Task.FromResult(0);
        }

        var toDeleteHashSet = fetchedDatasets.Select(ds => ds.Id).ToHashSet();
        
        return db.NasaDbSet
            .Where(d => toDeleteHashSet.Contains(d.Id))
            .ExecuteDeleteAsync(ct);
    }

    private async Task<int> InsertAsync(ICollection<NasaDataset> fetchedDatasets, CancellationToken ct)
    {
        var remoteIds = fetchedDatasets.Select(d => d.Id).ToHashSet();

        var existingIds = await db.NasaDbSet
            .AsNoTracking()
            .Where(d => remoteIds.Contains(d.Id))
            .Select(d => d.Id)
            .ToHashSetAsync(ct);
        
        var toInsert = fetchedDatasets
            .Where(d => !existingIds.Contains(d.Id))
            .ToList();

        if (toInsert.Count == 0)
        {
            return 0;
        }
        
        await db.BulkInsertAsync(
            toInsert,
            BulkConfig,
            cancellationToken: ct);

        return toInsert.Count;
    }
}