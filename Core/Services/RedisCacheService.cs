using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Core.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);
    Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions? options = null, CancellationToken token = default);
}

public class RedisCacheService(IDistributedCache cache): IRedisCacheService
{
    public async Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions? options = null, CancellationToken token = default)
    {
        var value = await Task.Run(() => JsonSerializer.Serialize(data), token);
        await cache.SetStringAsync(key, value, options ?? new (), token);
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