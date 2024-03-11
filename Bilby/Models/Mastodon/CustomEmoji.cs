using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record CustomEmoji
{
    [JsonPropertyName("shortcode")] public required string ShortCode { get; init; }
    [JsonPropertyName("url")] public required string Url { get; init; }
    [JsonPropertyName("static_url")] public required string StaticUrl { get; init; }
    [JsonPropertyName("visible_in_picker")] public bool VisibilityInPicker { get; init; } = true;
    [JsonPropertyName("category")] public required string Category { get; init; }
}
