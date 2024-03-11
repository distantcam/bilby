using System.Text.Json.Serialization;
using Bilby.ActivityStream;

namespace Bilby.Models.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<ASLink>))]
public class ASLink : Element
{
    public ASLink() : base(1, value1: [])
    {
        Context = "https://www.w3.org/ns/activitystreams";
        this["type"] = "Link";
    }

    public string Type => this["type"].AsString();

    public Element Context
    {
        get => this["@context"];
        set => this["@context"] = value;
    }

    public Element Href
    {
        get => this["href"];
        set => this["href"] = value;
    }
}
