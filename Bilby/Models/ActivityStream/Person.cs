using Bilby.Models.ActivityStream;
using System.Text.Json.Serialization;

namespace Bilby.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<Person>))]
public class Person : Actor
{
    public Person()
    {
        this["type"] = "Person";
    }
}
