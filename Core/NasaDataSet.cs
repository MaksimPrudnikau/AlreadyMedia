namespace Core;

public record Geolocation(
    string Type,
    double[] Coordinates
);

public record NasaDataset (
    string Name,
    string Id,
    string NameType,
    string RecClass,
    string Mass,
    string Fall,
    DateTime Year,
    string RecLat,
    string RecLong,
    Geolocation Geolocation
);