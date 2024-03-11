using System.Text.Json.Serialization;
using Bilby.ActivityStream;

namespace Bilby.Models.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<Endpoints>))]
public class Endpoints : Element
{
    public Endpoints() : base(1, value1: [])
    {
    }

    public Element ProxyUrl
    {
        get => this["proxyUrl"];
        set => this["proxyUrl"] = value;
    }

    public Element OauthAuthorizationEndpoint
    {
        get => this["oauthAuthorizationEndpoint"];
        set => this["oauthAuthorizationEndpoint"] = value;
    }

    public Element OauthTokenEndpoint
    {
        get => this["oauthTokenEndpoint"];
        set => this["oauthTokenEndpoint"] = value;
    }

    public Element ProvideClientKey
    {
        get => this["provideClientKey"];
        set => this["provideClientKey"] = value;
    }

    public Element SignClientKey
    {
        get => this["signClientKey"];
        set => this["signClientKey"] = value;
    }

    public Element SharedInbox
    {
        get => this["sharedInbox"];
        set => this["sharedInbox"] = value;
    }
}
