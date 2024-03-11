using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon.Enums;

public class EnumAsLowercaseStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String &&
            Enum.TryParse<TEnum>(reader.GetString()!, true, out var result))
            return result;

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString()!.ToLowerInvariant());
    }
}
