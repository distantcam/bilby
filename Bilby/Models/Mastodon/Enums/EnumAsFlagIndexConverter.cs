using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon.Enums;

public class EnumAsFlagIndexConverter<TEnum> : JsonConverter<TEnum> where TEnum : Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number &&
            reader.TryGetInt32(out var val))
            return (TEnum)(object)val;

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)(object)value);
    }
}
