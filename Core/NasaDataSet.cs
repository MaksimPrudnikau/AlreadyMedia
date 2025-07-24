namespace Core;

public class NasaDataSet
{
    public string Id { get; set; }
    
    public string? Center { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Photographer { get; set; }
    public string? Location { get; set; }
    public string? KeywordsJson { get; set; }
    
    public DateTime? DateCreated { get; set; }
    
    public string? MediaType { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FullResUrl { get; set; }
    
    public DateTime LastUpdated { get; set; } 
}