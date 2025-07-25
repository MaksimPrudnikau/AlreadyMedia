using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaHttpClient
{
    Task<ICollection<NasaDataset>> GetDatasetAsync();
}

public class NasaHttpClient(HttpClient httpClient, IOptions<NasaDatasetConfig> options, ILogger<NasaHttpClient> logger) : INasaHttpClient
{

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };
    
    public async Task<ICollection<NasaDataset>> GetDatasetAsync()
    {
        while (true)
        {
            logger.LogInformation("Starting NASA data fetch at: {time}", DateTimeOffset.Now);
            
            try
            {
                var dataset = await TryGetDatasetAsync();
                logger.LogInformation("Successfully fetched: {count} objects", dataset.Count);
                return dataset;
            }
            catch (HttpRequestException e)
            {
                logger.LogError("Error while fetching resource: {message}", e.Message);
                
                var resyncInterval = options.Value.ResyncIntervalSeconds;
                await Task.Delay(TimeSpan.FromSeconds(resyncInterval));
            }
        }
    }

    private async Task<ICollection<NasaDataset>> TryGetDatasetAsync()
    {
        var connection = options.Value.SourceUrl;

        var response = await httpClient.GetAsync(connection);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var dataSet = JsonSerializer.Deserialize<NasaDataset[]>(content, JsonSerializerOptions);
        return dataSet ?? [];
    }

}