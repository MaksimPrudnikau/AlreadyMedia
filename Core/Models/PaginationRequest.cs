using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class PaginationRequest
{
    [Display(Name = "Page Number")]
    [Range(0, int.MaxValue, ErrorMessage = "The {0} cannot be negative.")]
    public int Page { get; init; } = 0;
    
    [Display(Name = "Items Per Page")]
    [Range(1, 100, ErrorMessage = "The {0} must be between {1} and {2}.")]
    public int ItemsPerPage { get; init; } = 10;
}