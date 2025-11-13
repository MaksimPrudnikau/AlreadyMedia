using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Converters;

public class NullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss.fff";

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string for DateTime?, got {reader.TokenType}");

        var dateString = reader.GetString()!;

        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTime.TryParseExact(
                dateString,
                Format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
            return date;

        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
            return date;

        throw new JsonException($"Unable to parse date: '{dateString}'");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString(Format));
    }
}