using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Status
{
    // TODO

    public record Tag
    {
        [JsonPropertyName("name")] public required string Name { get; init; }
        [JsonPropertyName("url")] public required string Url { get; init; }
    }
}
