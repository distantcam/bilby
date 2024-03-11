using Bilby.Models.ActivityStream;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Bilby.ActivityStream.Element))]
[JsonSerializable(typeof(OrderedCollection))]
[JsonSerializable(typeof(Bilby.ActivityStream.Person))]

[JsonSerializable(typeof(Bilby.Features.ErrorResponse))]

[JsonSerializable(typeof(Bilby.Features.Client.Create.CreateRequest))]

[JsonSerializable(typeof(Bilby.Features.Micropub.JsonMicropub.HtmlContent))]
[JsonSerializable(typeof(Bilby.Features.Micropub.JsonMicropub.PhotoWithAlt))]
[JsonSerializable(typeof(Bilby.Features.Micropub.Query.ConfigResponse))]

[JsonSerializable(typeof(Bilby.Features.OAuth.Revoke.RevokeRequest))]
[JsonSerializable(typeof(Bilby.Features.OAuth.Token.TokenRequest))]
[JsonSerializable(typeof(Bilby.Features.OAuth.Token.TokenResponse))]

[JsonSerializable(typeof(Bilby.Features.WebFinger.NodeInfo.NodeLinks))]
[JsonSerializable(typeof(Bilby.Features.WebFinger.NodeInfoV20.ServerInfo))]
[JsonSerializable(typeof(Bilby.Features.WebFinger.WebFinger.UserDetails))]

[JsonSerializable(typeof(Bilby.Models.AuthCode))]
[JsonSerializable(typeof(Bilby.Models.MicroFormats2Object))]

[JsonSourceGenerationOptions(
    UseStringEnumConverter = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
