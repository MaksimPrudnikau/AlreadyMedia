using System.Text.Json;
using Core.Configs;
using Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaCacheService
{
    Task<NasaDatasetListResponse?> GetAsync(NasaDatasetListRequest request, CancellationToken token);
    void Save(NasaDatasetListRequest request, NasaDatasetListResponse response);

}

public class NasaCacheService(IDistributedCache cache, IOptions<NasaDatasetConfig> options): INasaCacheService
{
    public async Task<NasaDatasetListResponse?> GetAsync(NasaDatasetListRequest request, CancellationToken token = default)
    {
        var key = BuildKey(request);

        var value = await cache.GetStringAsync(key, token);
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<NasaDatasetListResponse?>(value);
    }

    public void Save(NasaDatasetListRequest request, NasaDatasetListResponse set)
    {
        var key = BuildKey(request);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.Value.SyncIntervalSeconds)
        };

        var value = JsonSerializer.Serialize(set);
        cache.SetStringAsync(key, value, cacheOptions);
    }

    private static string BuildKey(NasaDatasetListRequest request)
    {
        return string.Join("&",
            $"{nameof(request.FromYear)}=${request.FromYear}",
            $"{nameof(request.ToYear)}=${request.ToYear}",
            $"{nameof(request.RecClass)}=${request.RecClass}",
            $"{nameof(request.Page)}=${request.Page}",
            $"{nameof(request.ItemsPerPage)}=${request.ItemsPerPage}",
            $"{nameof(request.NameContains)}=${request.NameContains}"
        );
    }
}