using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Rule
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("text")] public required string Text { get; init; }
}
