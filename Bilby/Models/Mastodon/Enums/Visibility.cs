using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon.Enums;

[JsonConverter(typeof(EnumAsLowercaseStringConverter<Visibility>))]
public enum Visibility
{
    Public,
    Unlisted,
    Private,
    Direct,
}
