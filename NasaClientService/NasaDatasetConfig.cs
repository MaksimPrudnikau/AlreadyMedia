namespace NasaClientService;

public class NasaDatasetConfig
{
    public string? SourceUrl { get; init; }
    public int SyncIntervalSeconds { get; init; } = 30;

}