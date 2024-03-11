using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon.Enums;

[Flags]
[JsonConverter(typeof(FilterContextConverter))]
public enum FilterContext
{
    None = 0,
    Home = 1,
    Notifications = 2,
    Public = 4,
    Thread = 8
}

public class FilterContextConverter : JsonConverter<FilterContext>
{
    public override FilterContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var filterContext = FilterContext.None;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return filterContext;

                filterContext |= Enum.Parse<FilterContext>(reader.GetString()!, true);
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, FilterContext value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        if ((value & FilterContext.Home) != 0) writer.WriteStringValue("home");
        if ((value & FilterContext.Notifications) != 0) writer.WriteStringValue("notifications");
        if ((value & FilterContext.Public) != 0) writer.WriteStringValue("public");
        if ((value & FilterContext.Thread) != 0) writer.WriteStringValue("thread");
        writer.WriteEndArray();
    }
}
