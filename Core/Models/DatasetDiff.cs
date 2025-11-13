namespace Core.Models;

public record DatasetDiff(IList<NasaDataset> ToInsert, IList<int> ToDelete)
{
    public bool IsEmpty => !ToInsert.Any() && !ToDelete.Any();
};