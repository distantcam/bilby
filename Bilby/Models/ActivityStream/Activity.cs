using Bilby.Models.ActivityStream;
using System.Text.Json.Serialization;

namespace Bilby.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<Activity>))]
public class Activity : ASObject
{
    public Activity()
    {
        this["type"] = "Activity";
    }

    public Element Actor
    {
        get => this["actor"];
        set => this["actor"] = value;
    }
}
