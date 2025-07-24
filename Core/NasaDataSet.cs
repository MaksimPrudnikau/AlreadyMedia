using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core;

[Table("NasaDataset")]
public class NasaDataset
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string NameType { get; init; }
    public string RecClass { get; init; }
    public double Mass { get; init; }
    public string Fall { get; init; }

    private readonly DateTime _year;

    public DateTime Year
    {
        get => _year;
        init => _year = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
    }

    public double RecLat
    { get; init; }
    public double RecLong { get; init; }

    public Geolocation? Geolocation { get; init; }
}

[Owned]
public class Geolocation
{
    public string? Type { get; init; }
    public double[] Coordinates { get; init; } = [];
}