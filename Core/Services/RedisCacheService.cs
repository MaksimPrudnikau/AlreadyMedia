using System.Text.Json;
using Core.Configs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);
    Task SetAsync<T>(string key, T data, CancellationToken token = default);
}

public class RedisCacheService(IOptions<NasaDatasetConfig> config, IDistributedCache cache): IRedisCacheService
{
    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(config.Value.SyncIntervalSeconds)
    };

    public async Task SetAsync<T>(string key, T data, CancellationToken token = default)
    {
        var value = await Task.Run(() => JsonSerializer.Serialize(data), token);
        await cache.SetStringAsync(key, value, _options, token);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        var value = await cache.GetStringAsync(key, token);
        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return await Task.Run(() => JsonSerializer.Deserialize<T>(value), token);
    }
}