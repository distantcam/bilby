using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Marker
{
    [JsonPropertyName("last_read_id")] public required string LastReadId { get; init; }
    [JsonPropertyName("version")] public required int Version { get; init; }
    [JsonPropertyName("updated_at")] public required DateTime UpdatedAt { get; init; }
}
