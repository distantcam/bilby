using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Bilby.Models.MicroFormats2Object;

namespace Bilby.Models;

public record MicroFormats2Object(
    IEnumerable<string> Type,
    [property: JsonConverter(typeof(Microformats2PropertyConverter))]
    Dictionary<string, IEnumerable<JsonString>> Properties,
    string Action,
    string Url,
    [property: JsonConverter(typeof(Microformats2PropertyConverter))]
    Dictionary<string, IEnumerable<JsonString>>? Replace,
    [property: JsonConverter(typeof(Microformats2PropertyConverter))]
    Dictionary<string, IEnumerable<JsonString>>? Add,
    [property: JsonConverter(typeof(Microformats2PropertyConverter))]
    Dictionary<string, IEnumerable<JsonString>>? Delete
)
{
    public class Microformats2PropertyConverter : JsonConverter<Dictionary<string, IEnumerable<JsonString>>>
    {
        public override Dictionary<string, IEnumerable<JsonString>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var dictionary = new Dictionary<string, IEnumerable<JsonString>>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                        throw new JsonException();

                    var key = reader.GetString()!;
                    reader.Read();

                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException();

                    var values = new List<JsonString>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        values.Add(JsonString.Read(ref reader));
                    }
                    dictionary.Add(key, values);
                }
                return dictionary;
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var dictionary = new Dictionary<string, IEnumerable<JsonString>>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    dictionary.Add(reader.GetString()!, []);
                }
                return dictionary;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, IEnumerable<JsonString>> value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }

    public readonly struct JsonString
    {
        private readonly string _value;
        private JsonString(string value, bool isObject)
        {
            _value = value;
            IsObject = isObject;
        }

        public bool IsObject { get; }
        public override string ToString() => _value;

        public static implicit operator string(JsonString value) => value._value;
        public static explicit operator JsonString(string value) => new(value, false);

        public static JsonString Read(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
                return new(reader.GetString()!, false);

            // Everything else is converted to a json string
            var minifyOptions = new JsonWriterOptions
            {
                Indented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, minifyOptions);
            jsonDoc.WriteTo(writer);
            writer.Flush();
            return new(Encoding.UTF8.GetString(stream.ToArray()), true);
        }
    }
}
