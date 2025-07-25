using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Core.Models;

public class NasaDatasetListRequest
{
    [Required]
    [Range(2000, 2500)]
    public int? FromYear { get; init; }
    
    
    public int? ToYear { get; init; }
    
    [Required]
    public string? RecClass { get; init; }

    public int Page { get; init; } = 0;
    public int ItemsPerPage { get; init; } = 10;
}