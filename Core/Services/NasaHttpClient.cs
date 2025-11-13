using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Configs;
using Core.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Core.Services;

public interface INasaHttpClient
{
    Task<ICollection<NasaDataset>> GetDatasetAsync(CancellationToken ct = default);
}

public class NasaHttpClient(HttpClient httpClient, IOptions<NasaDatasetConfig> options, ILogger<NasaHttpClient> logger) : INasaHttpClient
{
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested)
        .Or<TimeoutException>()
        .Or<ArgumentNullException>()
        .Or<JsonException>()
        .Or<NotSupportedException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2, 4, 8... sec
            onRetry: (exception, timeSpan, attempt) =>
            {
                logger.LogWarning(
                    exception,
                    "NASA sync failed (attempt {Attempt}/3). Retrying in {Delay}s...",
                    attempt, timeSpan.TotalSeconds);
            });
    

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new NullableDateTimeJsonConverter() }
    };
    
    public async Task<ICollection<NasaDataset>> GetDatasetAsync(CancellationToken ct = default)
    {
        var dataset = await _retryPolicy.ExecuteAsync(TryGetDatasetAsync, ct);
        logger.LogInformation("Successfully fetched: {count} objects", dataset.Count);
        
        return dataset;

    }

    private async Task<ICollection<NasaDataset>> TryGetDatasetAsync(CancellationToken ct = default)
    {
        var connection = options.Value.SourceUrl;

        var response = await httpClient.GetAsync(connection, ct);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(ct);

        var dataSet = JsonSerializer.Deserialize<NasaDataset[]>(content, JsonSerializerOptions);
        return dataSet ?? [];
    }
}