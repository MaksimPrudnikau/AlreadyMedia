using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Configs;
using Microsoft.Extensions.Options;

namespace Core.Services;

public interface INasaHttpClient
{
    Task<ICollection<NasaDataset>> GetDatasetAsync();
}

public class NasaHttpClient(HttpClient httpClient, IOptions<NasaDatasetConfig> options) : INasaHttpClient
{

    private readonly static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };
    
    public async Task<ICollection<NasaDataset>> GetDatasetAsync()
    {
        return await TryGetDatasetAsync();
    }

    private async Task<ICollection<NasaDataset>> TryGetDatasetAsync()
    {
        try
        {
            var connection = options.Value.SourceUrl;
            
            // TODO: Доделать Retry
            var response = await httpClient.GetAsync(connection);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();

            var dataSet = JsonSerializer.Deserialize<NasaDataset[]>(content, JsonSerializerOptions);
            return dataSet ?? [];
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}