using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bilby.ActivityStream;

public partial class Element
{
    public class ElementJsonConverter<TElement> : JsonConverter<TElement> where TElement : Element, new()
    {
        public override TElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new TElement();

            if (reader.TokenType == JsonTokenType.String)
            {
                result.Convert(reader.GetString()!);
                return result;
            }

            if (reader.TokenType == JsonTokenType.True)
            {
                result.Convert(true);
                return result;
            }
            if (reader.TokenType == JsonTokenType.False)
            {
                result.Convert(false);
                return result;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                result.Convert(reader.GetInt32());
                return result;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var dictionary = new Dictionary<string, Element>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        result.Convert(dictionary);
                        return result;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }
                    var key = reader.GetString()!;
                    reader.Read();
                    var value = Read(ref reader, typeof(Element), options);
                    dictionary.Add(key, value);
                }
                throw new JsonException();
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = new List<Element>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        result.Convert(list);
                        return result;
                    }
                    var item = Read(ref reader, typeof(Element), options);
                    list.Add(item);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TElement value, JsonSerializerOptions options) =>
            WriteElement(writer, value, options);

        private void WriteElement(Utf8JsonWriter writer, Element value, JsonSerializerOptions options) =>
            value.Switch(
                o => WriteObject(writer, o, options),
                a => WriteArray(writer, a, options),
                writer.WriteStringValue,
                writer.WriteBooleanValue,
                writer.WriteNumberValue
            );

        private void WriteObject(Utf8JsonWriter writer, Dictionary<string, Element> obj, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var item in obj)
            {
                writer.WritePropertyName(item.Key);
                WriteElement(writer, item.Value, options);
            }
            writer.WriteEndObject();
        }

        private void WriteArray(Utf8JsonWriter writer, IEnumerable<Element> array, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in array)
            {
                WriteElement(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}
