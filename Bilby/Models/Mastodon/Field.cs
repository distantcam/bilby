using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Field
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("value")] public required string Value { get; init; }
    [JsonPropertyName("verified_at")] public DateTime? VerifiedAt { get; set; }
}
