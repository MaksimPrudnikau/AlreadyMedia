using System.Text.Json;
using System.Text.Json.Serialization;
using AlreadyMedia.Configs;
using Core;
using Microsoft.Extensions.Options;

namespace AlreadyMedia.Services;

public interface INasaDatasetClient
{
    Task<ICollection<NasaDataset>> GetDatasetAsync();
}

public class NasaDatasetClient(HttpClient httpClient, IOptions<NasaDatasetConfig> options) : INasaDatasetClient
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