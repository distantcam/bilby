using System.Text.Json.Serialization;
using Bilby.ActivityStream;

namespace Bilby.Models.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<ASObject>))]
public class ASObject : Element
{
    public ASObject() : base(1, value1: [])
    {
        Context = "https://www.w3.org/ns/activitystreams";
        this["type"] = "Object";
    }

    public string Type => this["type"].AsString();

    public Element Context
    {
        get => this["@context"];
        set => this["@context"] = value;
    }

    public Element Id
    {
        get => this["id"];
        set => this["id"] = value;
    }

    public Element Name
    {
        get => this["name"];
        set => this["name"] = value;
    }

    public Element Summary
    {
        get => this["summary"];
        set => this["summary"] = value;
    }
}
