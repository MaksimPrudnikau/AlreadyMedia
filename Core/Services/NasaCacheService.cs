using Core.Configs;
using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaCacheService
{
    NasaDatasetListResponse? Get(NasaDatasetListRequest request);
    void Save(NasaDatasetListRequest request, NasaDatasetListResponse response);

}

public class NasaCacheService(IMemoryCache cache, IOptions<NasaDatasetConfig> options): INasaCacheService
{
    public NasaDatasetListResponse? Get(NasaDatasetListRequest request)
    {
        var key = BuildKey(request);
        
        cache.TryGetValue<NasaDatasetListResponse>(key, out var value);

        return value;
    }

    public void Save(NasaDatasetListRequest request, NasaDatasetListResponse set)
    {
        var key = BuildKey(request);
        var cacheOptions = new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.Value.SyncIntervalSeconds)
        };
        
        cache.Set(key, set, cacheOptions);
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