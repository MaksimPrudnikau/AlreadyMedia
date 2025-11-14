namespace Core.Models;

public record SyncResult(int Added = 0, int Removed = 0, int Updated = 0)
{
    public static SyncResult operator +(SyncResult left, SyncResult right)
    {
        return new (
            Added: left.Added + right.Added,
            Removed: left.Removed + right.Removed,
            Updated: left.Updated + right.Updated
        );
    }

    public bool IsEmpty => Added == 0 && Removed == 0 && Updated == 0;
};