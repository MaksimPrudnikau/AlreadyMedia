using System.Runtime.CompilerServices;
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
    Task<IAsyncEnumerable<ICollection<NasaDataset>>> GetBatchedDatasetsAsync(int batchSize, CancellationToken ct = default);
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
    
    public async Task<IAsyncEnumerable<ICollection<NasaDataset>>> GetBatchedDatasetsAsync(int batchSize, CancellationToken ct = default)
    {
        var stream = await _retryPolicy.ExecuteAsync(FetchDatasetStreamAsync, ct);
        return StreamBatchedDatasetsAsync(stream, batchSize, ct);
    }

    private async IAsyncEnumerable<ICollection<NasaDataset>> StreamBatchedDatasetsAsync(
        Stream stream, 
        int batchSize, 
        [EnumeratorCancellation]CancellationToken ct = default)
    {
        logger.LogInformation("Starting sync with with {BatchSize} batch size", batchSize);
        
        var datasets = JsonSerializer.DeserializeAsyncEnumerable<NasaDataset>(stream, JsonSerializerOptions, ct);
        var batch = new List<NasaDataset>();
        
        await foreach (var dataset in datasets)
        {
            if (dataset is null)
            {
                continue;
            }
            
            if (batch.Count < batchSize)
            {
                batch.Add(dataset);
                continue;
            }

            yield return batch;
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }
    

    private async Task<Stream> FetchDatasetStreamAsync(CancellationToken ct = default)
    {
        var connection = options.Value.SourceUrl;

        var response = await httpClient.GetAsync(connection, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(ct);
    }
}