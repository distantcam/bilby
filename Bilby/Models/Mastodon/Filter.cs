using System.Text.Json.Serialization;
using Bilby.Models.Mastodon.Enums;

namespace Bilby.Models.Mastodon;

public record Filter
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
    [JsonPropertyName("context")] public required FilterContext Context { get; init; }
    [JsonPropertyName("expires_at")] public DateTime? ExpiresAt { get; init; }
    [JsonPropertyName("filter_action")] public Action FilterAction { get; init; } = Action.Warn;

    [JsonPropertyName("keywords")] public IEnumerable<FilterKeyword>? Keywords { get; init; }
    [JsonPropertyName("statuses")] public IEnumerable<FilterStatus>? Statuses { get; init; }


    [JsonConverter(typeof(EnumAsLowercaseStringConverter<Action>))]
    public enum Action
    {
        Warn,
        Hide
    }

    public record FilterKeyword
    {
        [JsonPropertyName("id")] public required string Id { get; init; }
        [JsonPropertyName("keyword")] public required string Keyword { get; init; }
        [JsonPropertyName("whole_word")] public required bool WholeWord { get; init; }
    }

    public record FilterStatus
    {
        [JsonPropertyName("id")] public required string Id { get; init; }
        [JsonPropertyName("status_id")] public required string StatusId { get; init; }
    }
}
