using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record Announcement
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("content")] public required string Content { get; init; }
    [JsonPropertyName("starts_at")] public DateTime? StartsAt { get; init; }
    [JsonPropertyName("ends_at")] public DateTime? EndsAt { get; init; }
    [JsonPropertyName("published")] public required bool Published { get; init; }
    [JsonPropertyName("all_day")] public bool AllDay { get; init; }
    [JsonPropertyName("published_at")] public required DateTime PublishAt { get; init; }
    [JsonPropertyName("updated_at")] public required DateTime UpdatedAt { get; init; }
    [JsonPropertyName("read")] public bool? Read { get; init; }

    [JsonPropertyName("mentions")] public IEnumerable<AnnouncementAccount>? Mentions { get; init; }
    [JsonPropertyName("statuses")] public IEnumerable<AnnouncementStatus>? Statuses { get; init; }
    [JsonPropertyName("tags")] public IEnumerable<Status.Tag>? Tags { get; init; }
    [JsonPropertyName("emojis")] public IEnumerable<CustomEmoji>? Emojis { get; init; }
    [JsonPropertyName("reactions")] public IEnumerable<Reaction>? Reactions { get; init; }

    public record AnnouncementAccount
    {
        [JsonPropertyName("id")] public required string Id { get; init; }
        [JsonPropertyName("username")] public required string Username { get; init; }
        [JsonPropertyName("acct")] public required string AccountName { get; init; }
        [JsonPropertyName("url")] public required string Url { get; init; }
    }

    public record AnnouncementStatus
    {
        [JsonPropertyName("id")] public required string Id { get; init; }
        [JsonPropertyName("url")] public required string Url { get; init; }
    }
}
