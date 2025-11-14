using Core.Configs;
using Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaCacheService
{
    Task<NasaDatasetListResponse?> GetAsync(NasaDatasetListRequest request, CancellationToken ct = default);
    Task SaveAsync(NasaDatasetListRequest request, NasaDatasetListResponse response, CancellationToken ct = default);

}

public class NasaCacheService(IRedisCacheService cache, IOptions<NasaDatasetConfig> options): INasaCacheService
{
    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.Value.SyncIntervalSeconds)
    };
    
    public async Task<NasaDatasetListResponse?> GetAsync(NasaDatasetListRequest request, CancellationToken ct = default)
    {
        var key = BuildKey(request);

        return await cache.GetAsync<NasaDatasetListResponse?>(key, ct);
    }

    public async Task SaveAsync(NasaDatasetListRequest request, NasaDatasetListResponse set, CancellationToken ct = default)
    {
        var key = BuildKey(request);
        await cache.SetAsync(key, set, _options, ct);

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