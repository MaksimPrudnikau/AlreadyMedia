namespace Core.Configs;

public class NasaDatasetConfig
{
    public string? SourceUrl { get; init; }
    public int SyncIntervalSeconds { get; init; } = 30;
    public int MaxRetries { get; init; } = 3;

}