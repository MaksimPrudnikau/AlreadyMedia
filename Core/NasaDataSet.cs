using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Converters;
using Microsoft.EntityFrameworkCore;

namespace Core;

[Table("NasaDataset")]
public class NasaDataset
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? NameType { get; init; }
    public string? RecClass { get; init; }
    public double? Mass { get; init; }
    public string? Fall { get; init; }

    [JsonPropertyName("Year")]
    [Column(TypeName = "timestamp")]
    [JsonConverter(typeof(NullableDateTimeJsonConverter))]
    public DateTime? Date { get; init; }

    public double RecLat { get; init; }
    public double RecLong { get; init; }

    public Geolocation? Geolocation { get; init; }
}

[Owned]
public class Geolocation
{
    public string? Type { get; init; }
    public double[] Coordinates { get; init; } = [];
}