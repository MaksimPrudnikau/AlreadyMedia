using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services;

public interface INasaBackgroundService
{
    Task<(int removed, int added)> UpsertDatasetsAsync();
}

public class NasaBackgroundService : INasaBackgroundService
{
    private readonly INasaHttpClient _nasaClient;
    private readonly IDistributedCache _cacheService;
    private readonly AppDbContext _appDbContext;

    public NasaBackgroundService(INasaHttpClient nasaClient, IServiceProvider serviceProvider, IDistributedCache cacheService)
    {
        _nasaClient = nasaClient;
        _cacheService = cacheService;

        var scope = serviceProvider.CreateScope();
        _appDbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
    }

    public async Task<(int removed, int added)> UpsertDatasetsAsync()
    {
        var remoteDatasets = await FetchRemoteDatasetsAsync();
        return await SyncWithDatabaseAsync(remoteDatasets);
    }

    private async Task<IDictionary<int, NasaDataset>> FetchRemoteDatasetsAsync()
    {
        var datasets = await _nasaClient.GetDatasetAsync();
        return datasets.ToDictionary(dataset => dataset.Id);
    }

    private async Task<(int removed, int added)> SyncWithDatabaseAsync(IDictionary<int, NasaDataset> remoteDatasets)
    {
        var existingDatasets = await GetFromCacheAsync();
        var isCached = existingDatasets is not null;
        existingDatasets ??= await GetExistingDatasetsAsync(remoteDatasets.Keys);
        
        var removed = await RemoveStaleDatasetsAsync(remoteDatasets);
        var added = await AddNewDatasetsAsync(remoteDatasets, existingDatasets);

        if (!isCached || removed > 0 || added > 0)
        {
            var value = JsonSerializer.Serialize(existingDatasets);
            await _cacheService.SetStringAsync("data", value);
        }
    
        await _appDbContext.SaveChangesAsync();

        return (removed, added);
    }

    private async Task<IList<NasaDataset>?> GetFromCacheAsync()
    {
        var value = await _cacheService.GetStringAsync("data");
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return JsonSerializer.Deserialize<IList<NasaDataset>>(value);
    }

    private async Task<IList<NasaDataset>> GetExistingDatasetsAsync(ICollection<int> remoteDatasetIds)
    {
        return await _appDbContext.NasaDbSet
            .Where(dataset => remoteDatasetIds.Contains(dataset.Id))
            .ToListAsync();
    }

    private async Task<int> RemoveStaleDatasetsAsync(IDictionary<int, NasaDataset> remoteDatasets)
    {
        var remoteDatasetIds = remoteDatasets.Keys.ToHashSet();
        var staleDatasets = await _appDbContext.NasaDbSet
            .Where(dataset => !remoteDatasetIds.Contains(dataset.Id))
            .ToListAsync();

        if (staleDatasets.Count != 0)
        {
            _appDbContext.NasaDbSet.RemoveRange(staleDatasets);
        }

        return staleDatasets.Count;
    }


    private async Task<int> AddNewDatasetsAsync(
        IDictionary<int, NasaDataset> remoteDatasets,
        IList<NasaDataset> existingDatasets)
    {
        var existingDatasetIds = existingDatasets.Select(d => d.Id).ToHashSet();
        var newDatasets = remoteDatasets.Values
            .Where(dataset => !existingDatasetIds.Contains(dataset.Id))
            .ToList();

        if (newDatasets.Count != 0)
        {
            await _appDbContext.NasaDbSet.AddRangeAsync(newDatasets);
        }

        return newDatasets.Count;
    }
}