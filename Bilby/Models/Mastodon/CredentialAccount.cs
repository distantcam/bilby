using System.Text.Json.Serialization;
using Bilby.Models.Mastodon.Enums;

namespace Bilby.Models.Mastodon;

public record CredentialAccount : Account
{
    [JsonPropertyName("source")] public required SourceHash Source { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("role")] public Role? Role { get; init; }

    public record SourceHash
    {
        [JsonPropertyName("note")] public required string Note { get; init; }
        [JsonPropertyName("fields")] public IEnumerable<Field> Fields { get; init; } = [];
        [JsonPropertyName("privacy")] public Visibility Privacy { get; init; }
        [JsonPropertyName("sensitive")] public bool Sensitive { get; init; }
        [JsonPropertyName("language")] public string Language { get; init; } = "";
        [JsonPropertyName("follow_requests_count")] public int FollowRequestsCount { get; init; }
    }
}
