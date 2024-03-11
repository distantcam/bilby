using System.Text.Json.Serialization;
using Bilby.Models.Mastodon.Enums;

namespace Bilby.Models.Mastodon;

public record Role
{
    [JsonPropertyName("sensitive")] public required int Id { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("color")] public required string Colour { get; init; }
    [JsonPropertyName("permissions")] public required Permission Permissions { get; init; }
    [JsonPropertyName("highlighted")] public required bool Highlighted { get; init; }
}
