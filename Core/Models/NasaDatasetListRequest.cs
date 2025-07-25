using System.ComponentModel.DataAnnotations;

namespace Core.Models;

public class NasaDatasetListRequest: PaginationRequest, IValidatableObject
{
    [Display(Name = "From Year")]
    [Range(1, 9999, ErrorMessage = "The {0} must be between {1} and {2}.")]
    public int? FromYear { get; init; }
    
    [Display(Name = "To Year")]
    [Range(1, 9999, ErrorMessage = "The {0} must be between {1} and {2}.")]
    public int? ToYear { get; init; }
    
    [Display(Name = "Record Class")]
    [StringLength(50, ErrorMessage = "The {0} cannot exceed {1} characters.")]
    public string? RecClass { get; init; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromYear > ToYear)
        {
            yield return new(
                "Selected years are in reverse order.",
                [nameof(FromYear), nameof(ToYear)]);
        }
    }
}