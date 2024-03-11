using System.Text.Json.Serialization;
using Bilby.ActivityStream;

namespace Bilby.Models.ActivityStream;

// https://www.w3.org/TR/activitypub/#actor-objects
[JsonConverter(typeof(ElementJsonConverter<Actor>))]
public class Actor : ASObject
{
    public Actor()
    {
        this["type"] = "Actor";
    }

    public Element Inbox
    {
        get => this["inbox"];
        set => this["inbox"] = value;
    }

    public Element Outbox
    {
        get => this["outbox"];
        set => this["outbox"] = value;
    }

    public Element Following
    {
        get => this["following"];
        set => this["following"] = value;
    }

    public Element Followers
    {
        get => this["followers"];
        set => this["followers"] = value;
    }

    public Element Liked
    {
        get => this["liked"];
        set => this["liked"] = value;
    }

    public Element Streams
    {
        get => this["streams"];
        set => this["streams"] = value;
    }

    public Element PreferredUsername
    {
        get => this["preferredUsername"];
        set => this["preferredUsername"] = value;
    }

    public Endpoints Endpoints
    {
        get => (Endpoints)this["endpoints"];
        set => this["endpoints"] = value;
    }
}
