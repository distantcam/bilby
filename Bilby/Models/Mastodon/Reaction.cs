using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Reaction
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("count")] public required int Count { get; init; }
    [JsonPropertyName("me")] public bool? Me { get; init; }
    [JsonPropertyName("url")] public string? Url { get; init; }
    [JsonPropertyName("static_url")] public string? StaticUrl { get; init; }
}
