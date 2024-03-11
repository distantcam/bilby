using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Application
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("website")] public string? Website { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("client_id")] public string? ClientId { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("client_secret")] public string? ClientSecret { get; init; }
}
