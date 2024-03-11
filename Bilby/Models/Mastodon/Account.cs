using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Account
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("username")] public required string Username { get; init; }
    [JsonPropertyName("acct")] public required string AccountName { get; init; }
    [JsonPropertyName("url")] public required string Url { get; init; }
    [JsonPropertyName("display_name")] public required string DisplayName { get; init; }
    [JsonPropertyName("note")] public required string Note { get; init; }
    [JsonPropertyName("avatar")] public required string Avatar { get; init; }
    [JsonPropertyName("avatar_static")] public required string AvatarStatic { get; init; }
    [JsonPropertyName("header")] public required string Header { get; init; }
    [JsonPropertyName("header_static")] public required string HeaderStatic { get; init; }

    [JsonPropertyName("locked")] public bool Locked { get; init; }
    [JsonPropertyName("bot")] public bool Bot { get; init; }
    [JsonPropertyName("group")] public bool Group { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("discoverable")] public bool? Discoverable { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("noindex")] public bool? NoIndex { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("suspended")] public bool Suspended { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("limited")] public bool Limited { get; init; }

    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("last_status_at")] public DateTime? LastStatusAt { get; init; }

    [JsonPropertyName("statuses_count")] public int StatusesCount { get; init; }
    [JsonPropertyName("followers_count")] public int FollowersCount { get; init; }
    [JsonPropertyName("following_count")] public int FollowingCount { get; init; }

    [JsonPropertyName("fields")] public IEnumerable<Field> Fields { get; init; } = [];
    [JsonPropertyName("emojis")] public IEnumerable<CustomEmoji> Emojis { get; init; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("moved")] public Account? Moved { get; init; }
}
