namespace Core.Models;

public class NasaDatasetListResponse
{
    public required PaginationModel Pagination { get; init; }
    public required IEnumerable<NasaDatasetGroupedModel> Dataset { get; init; }
}